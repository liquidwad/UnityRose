'use strict';

var crypto = require('crypto'),
	type = require('../type'),
	opcodes = require('./opcodes');

module.exports = function(response, numChars) 
{
	var packet = {
		type: type.User,
		operation: opcodes.userOperation.Login,
		response: {
			status: response,
			numChars: numChars
		}
	};

	return packet;
};