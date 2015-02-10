'use strict';

var crypto = require('crypto'),
	type = require('../type'),
	opcodes = require('./opcodes');

module.exports = function(response) 
{
	var packet = {
		type: type.User.value,
		operation: opcodes.userOperation.Register.value,
		response: response
	};

	return packet;
};