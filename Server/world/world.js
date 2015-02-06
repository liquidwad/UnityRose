'use strict';

var UserManager = require('./usermanager'),
	MapManager = require('./mapmanager');

function World() {

	this.userManager = new UserManager();

	this.mapManager = new MapManager();

	console.log("World has started");
};

module.exports = World;