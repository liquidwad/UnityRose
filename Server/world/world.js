'use strict';

var UserManager = require('./usermanager'),
	MapManager = require('./mapmanager');

function World() {

	this.userManager = new UserManager();

	this.mapManager = new MapManager();

	this.handleUserPacket = function(client, packet) {
		this.userManager.handlePacket(client, packet);
	};

	this.handleCharacterPacket = function(client, packet) {
		//probably move mapmanager to world
		var clientMoveData = crypto.encrypt(data);
		client.write(clientMoveData);
	};

	this.removeClient = function(client) {
		//remove from usermanager and world
	} 

	console.log("World has started");
};

module.exports = World;