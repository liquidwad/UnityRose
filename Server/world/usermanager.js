'use strict';

var _ = require('lodash'),
	User = require('./user');

var UserManager = function() {

	this.users = [];

	this.getUser = function(socket) {

		var index = _.findIndex(users, function(user) {
			return (user.socket === socket);
		});

		if(index < 0) {
			throw "User with this socket doesn't exist";
		}

		return this.users[index];
	};


	this.newUser = function(socket, clientId) {
		var user = {};

		user.data = new User(clientId);

		user.socket = socket;

		users.push(user);

		return user;
	};

	console.log("User manager has loaded");
};


module.exports = UserManager;