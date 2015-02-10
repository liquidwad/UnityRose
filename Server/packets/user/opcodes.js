'use strict';

var Enum = require('enum');

module.exports = {
	userOperation: new Enum({
		'Register': 0,
		'Login': 1,
		'CharSelect' : 2
	}),

	//LOGIN
	loginCallbackOperation: new Enum({
		'Error': 0,
		'NotExist': 1,
		'Valid': 2
	}),

	//REGISTER
	registerCallbackOperation: new Enum({
		'Error': 0,
		'Exists': 1,
		'Valid': 2
	})
};