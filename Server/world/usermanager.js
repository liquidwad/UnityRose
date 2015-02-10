'use strict';

var _ = require('lodash'),
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
		console.log("Handle Login Packet");
		UserModel.findOne({ 
			username: packet.username, 
			password: packet.password 
		}, function(err, user) {

			var response;

			if(err) {
				response = opcodes.loginCallbackOperation.Error;
			} else {
				if(user) {
					this.addUser(client, user);
					response = opcodes.loginCallbackOperation.Valid;
				} else {
					response = opcodes.loginCallbackOperation.NotExist;
				}
			}

			var encryptedPacket = crypto.encrypt( loginPacket( response ) );

			client.write( encryptedPacket );
		});
	};

	this.registerUser = function(client, packet) {
		//register user
		console.log("Handle Register Packet");

		UserModel.findOne({
			$or: [
				{ 'username': packet.username },
				{ 'email': packet.email }
			]
		}, function(err, user) {

			var response;

			if(err) {
				response = opcodes.registerCallbackOperation.Error;
			} 
			else if(user) {
				response = opcodes.registerCallbackOperation.Exists;
			}

			//send if error or exists
			if(response !== undefined) {
				var encryptedPacket = crypto.encrypt( registerPacket( response ) );
				client.write(encryptedPacket);
				return;
			}

			UserModel.create({
				'username': packet.username,
				'password': packet.password,
				'email': packet.email
			}, function(err) {
				if(err) {
					response = opcodes.registerCallbackOperation.Error;
				} else {
					response = opcodes.registerCallbackOperation.Valid;
				}

				console.log("Created User");

				var encryptedPacket = crypto.encrypt( registerPacket( response ) );
				client.write(encryptedPacket);
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
	}


	this.addUser = function(client, model) {
		var user = new User(client, model);
		users.push(user);
		return user;
	};

	this.disconnected = function(client) {
		this.removeUser(client);
	};

	this.registerPacket = function(key, func) {
		this.packetHandlers[key] = func;
	};

	this.registerPacket(opcodes.userOperation.Register, this.registerUser);
	this.registerPacket(opcodes.userOperation.Login, this.loginUser);

	console.log("User manager has loaded");
};


module.exports = UserManager;