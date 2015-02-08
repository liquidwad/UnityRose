'use strict';

var Enum = require('enum');

module.exports = {

	packetType: new Enum({
		'User': 1,
		'Character': 2
	}),

	userOperation: new Enum({
		'Login': 1
	}),


	characterOperation: new Enum({
		'Walk': 1
	}),


	User: 'userOperation',
	Character: 'characterOperation'
};