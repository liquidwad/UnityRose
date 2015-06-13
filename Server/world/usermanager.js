'use strict';

var _ = require('lodash'),
	validator = require('validator'),
	mongoose = require('mongoose'),
	UserModel = require('../models/user'),
	User = require('./user'),
	loginPacket = require('../packets/user/loginpacket'),
	registerPacket = require('../packets/user/registerpacket'),
	opcodes = require('../packets/opcodes'),
	crypto = require('../crypto');

var UserManager = function() {

	this.users = [];

	this.packetHandlers = {};

	this.handlePacket = function(client, packet) {

		var packetHandler = this.packetHandlers[packet.operation];

		if(packetHandler !== undefined) {
			packetHandler(client, packet);
		}
	};

	this.loginUser = function(client, packet) {
		var userManager = this;

		UserModel.findOne({ 
			username: packet.username, 
			password: packet.password 
		}, function(err, user) {

			var response;

			if(err) {
				response = opcodes.loginCallbackOperation.Error;
			} else {
				if(user) {
					userManager.addUser(client, user);
					response = opcodes.loginCallbackOperation.Valid;
				} else {
					response = opcodes.loginCallbackOperation.NotExist;
					
				}
			}

			var encryptedPacket = crypto.encrypt( loginPacket( response, user.numChars ) );
			client.write( encryptedPacket );
		});
	};


	this.sendRegistrationResponse = function(client, response) {
		var encryptedPacket = crypto.encrypt( registerPacket( response ) );
		client.write(encryptedPacket);
	}

	this.registerUser = function(client, packet) {

		//validate username length
		if(!validator.isLength(packet.username, 5, 20)) {
			this.sendRegistrationResponse(client, opcodes.registerCallbackOperation.UsernameTooShort);
			return;
		}

		//validate username bad characters
		if(!validator.isAlphanumeric(packet.username)) {
			this.sendRegistrationResponse(client, opcodes.registerCallbackOperation.UsernameBadChars);
			return;
		}

		//validate password length
		if(!validator.isLength(packet.password, 6, 20)) {
			this.sendRegistrationResponse(client, opcodes.registerCallbackOperation.PasswordTooShort);
		}

		//validate email
		if(!validator.isEmail(packet.email)) {
			this.sendRegistrationResponse(client, opcodes.registerCallbackOperation.EmailInvalid);
		}

		var userManager = this;

		UserModel.findOne({
			$or: [
				{ 'username': packet.username },
				{ 'email': packet.email }
			]
		}, 
		function(err, user) {

			var response;

			if(err) {
				response = opcodes.registerCallbackOperation.Error;
			} 
			else if(user) {
				if(user.username == packet.username) {
					response = opcodes.registerCallbackOperation.UsernameExists;
				}
				else {
					response = opcodes.registerCallbackOperation.EmailUsed;
				}
			}

			//if username/email exists or error respond
			if(response !== undefined) {
				userManager.sendRegistrationResponse(client, response);
				return;
			}

			//register user
			UserModel.create({
				'username': packet.username,
				'password': packet.password,
				'email': packet.email
			}, function(err, user) {
				if(err) {
					response = opcodes.registerCallbackOperation.Error;
				} else {
					response = opcodes.registerCallbackOperation.Valid;
				}

				userManager.sendRegistrationResponse(client, response);
			});
		});
	};

	this.findIndex = function(client) {
		var index = _.findIndex(users, function(user) {
			return (user.client === client);
		});

		if(index < 0) {
			throw "User with this socket doesn't exist";
		}

		return index;
	}

	this.getUser = function(client) {
		var index = this.findIndex(client);
		return this.users[index];
	};

	this.removeUser = function(client) {
		_.remove(this.users, function(user) {
			return user.client === client;
		});
	};

	
	this.addUser = function(client, model) {
		var user = new User(client, model);
		this.users.push(user);
		return user;
	};

	this.disconnected = function(client) {
		this.removeUser(client);
	};

	this.registerPacket = function(key, func) {
		//if(process.env.NODE_ENV == "production") {
			//this.packetHandlers[key] = _.bind(func, this);
		//}

		//only for debugging
		//if(process.env.NODE_ENV == "development") {}
		var _this = this;
		this.packetHandlers[key] = function(client, packet) {

			var wrapper = _.bind(func, _this),
					operation;

			_.find(opcodes.userOperation, function(value, name) {
				if(packet.operation === value) {
					operation = name;
					return true;
				}
			});

			console.log("[SERVER] < [" + client.id + "] - '" + operation + "' packet recieved");

			wrapper(client, packet);

			console.log("[SERVER] > [" + client.id + "] - '" + operation + "' packet handled");
		};
	};

	this.registerPacket(opcodes.userOperation.Register, this.registerUser);
	this.registerPacket(opcodes.userOperation.Login, this.loginUser);

	console.log("User manager has loaded");
};

module.exports = UserManager;