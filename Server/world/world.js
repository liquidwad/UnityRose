'use strict';

var _ = require('lodash'),
	UserManager = require('./usermanager'),
	crypto = require('../crypto'),
	MapManager = require('./mapmanager');

function World() {

	this.userManager = new UserManager();

	this.mapManager = new MapManager();

	this.handleUserPacket = function(client, packet) {
		this.userManager.handlePacket(client, packet);
	};

	this.handleCharacterPacket = function(clients, packet) {
		
		//probably move mapmanager to world
		var clientMoveData = crypto.encrypt(packet);
			
		_.each(clients, function(client) {
			client.write(clientMoveData);
		});
		
	};

	this.writeMoveData = function(client, moveData) {
		client.write(moveData);
	}
	this.removeClient = function(client) {
		//remove from usermanager and world
	} 

	console.log("World has started");
};

module.exports = World;