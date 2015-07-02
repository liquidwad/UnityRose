'use strict';

var _ = require('lodash'),
	validator = require('validator'),
	mongoose = require('mongoose'),
	UserModel = require('../models/user'),
	CharModel = require('../models/char'),
	User = require('./user'),
	loginPacket = require('../packets/user/loginpacket'),
	registerPacket = require('../packets/user/registerpacket'),
	CharSelectPackets = require('../packets/user/CharSelectPackets'),
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
					user.online = true;
					userManager.addUser(client, user);
					response = opcodes.loginCallbackOperation.Valid;
				} else {
					response = opcodes.loginCallbackOperation.NotExist;
					
				}
			}

			var encryptedPacket = crypto.encrypt( loginPacket( response) );
			client.write( encryptedPacket );
		});
	};

	
	// Sends a packet for each character the user owns to the client
	// With a character model for each
	this.spawnChars = function(client, packet) {
		var userManager = this;
		
		var user = userManager.getUser( client );
		if( user == null )
		{
			console.log("User is null");
			return;
		}
		
		UserModel.findOne({
			username: user.username 
		}, function(err, foundUser) {
			if(err)
			{
				console.log("Couldn't find user: " + user.username);
				return;
			}
			
			// For each char ID found in user
			_.each(foundUser._chars, function(char) {
				console.log("char: " + char);
				// Find the char with this char ID in the char database
				CharModel.findOne({ 
					name: char
				}, function(err, foundChar) {
		
					if(!err && foundChar)
					{
						var encryptedPacket = crypto.encrypt( CharSelectPackets.SpawnCharPacket( foundChar ) );
						client.write( encryptedPacket );
					}
					else
					{
						console.log("Couldn't find char: " + char);
					}
					
				}); // end findOne CharModel
			});  // end each
		}); // end findOne UserModel
	};
	
	
	this.createChar = function(client, packet ){
		var userManager = this;
		var user = userManager.getUser( client );
		if( user == null )
			return;
		
		// TODO: handle the case where user already has max number of chars
		// validate the new character
		var validHairs = [ 0, 5, 10, 15, 20, 25, 30 ];
		var validFaces = [1, 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92];
		var validGenders = ["MALE", "FEMALE"];
		
		var hairValid = validHairs.indexOf(packet.charModel.equip.hairID ) >= 0;
		var faceValid = validFaces.indexOf(packet.charModel.equip.faceID ) >= 0;
		var genderValid = validGenders.indexOf(packet.charModel.gender ) >= 0;
		
		if( ! (hairValid && faceValid && genderValid ) )
		{
			client.write(crypto.encrypt( CharSelectPackets.CreateCharPacket( opcodes.charSelectCallBackOp.InvalidChoice ) ) );
			return;	
		}
		
		CharModel.findOne({
			name: packet.charModel.name
		}, function(err, char) {
			if( !err && char )
			{
				client.write(crypto.encrypt( CharSelectPackets.CreateCharPacket( opcodes.charSelectCallBackOp.NameExists ) ) );
				return;		
			}
			
			CharModel.create({
				'name': packet.charModel.name,
				'gender': packet.charModel.gender,
				'equip': {
					'hairID': packet.charModel.equip.hairID,
					'faceID' : packet.charModel.equip.faceID	
				}
			
			}, function(err, char) {
				if(!err && char )
				{
					UserModel.update (
						{ username: user.username },
						{ $push: { _chars: packet.charModel.name} }
					);

					client.write(crypto.encrypt( CharSelectPackets.CreateCharPacket( opcodes.charSelectCallBackOp.Success) ) );
				}
				else
					client.write(crypto.encrypt( CharSelectPackets.CreateCharPacket( opcodes.charSelectCallBackOp.Error ) ) );
					
			});
			
		
		});
	};
	
	
	this.sendRegistrationResponse = function(client, response) {
		var encryptedPacket = crypto.encrypt( registerPacket( response ) );
		client.write(encryptedPacket);
	};

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
		var index = _.findIndex(this.users, function(user) {
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
	this.registerPacket(opcodes.userOperation.CharSelect, this.spawnChars);
	this.registerPacket(opcodes.userOperation.SelectChar, this.selectChar);
	this.registerPacket(opcodes.userOperation.CreateChar, this.createChar);
	this.registerPacket(opcodes.userOperation.DeleteChar, this.deleteChar);
	console.log("User manager has loaded");
};

module.exports = UserManager;