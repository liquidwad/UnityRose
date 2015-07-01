'use strict';

var crypto = require('crypto'),
	type = require('../type'),
	opcodes = require('./opcodes');

module.exports = function(model) 
{
	var packet = {
		type: type.User,
		operation: opcodes.userOperation.SpawnChar,
		charModel: model
	};

	return packet;
};