'use strict';

module.exports = {
	userOperation: {
		Register: 0,
		Login: 1,
		CharSelect: 2,
		SpawnChar: 3
	},

	//LOGIN
	loginCallbackOperation: {
		Error: 0,
		NotExist: 1,
		Valid: 2
	},

	//REGISTER
	registerCallbackOperation: {
		Error: 0,
		Valid: 1,
		UsernameTooShort: 2,
		UsernameBadChars: 3,
		UsernameExists: 4,
		PasswordTooShort: 5,
		EmailUsed: 6,
		EmailInvalid: 7
	},
};