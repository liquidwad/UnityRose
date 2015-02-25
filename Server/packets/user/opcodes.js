'use strict';

module.exports = {
	userOperation: {
		Register: 0,
		Login: 1,
		CharSelect: 2
	},

	//LOGIN
	loginCallbackOperation: {
		Error: 0,
		NotExist: 1,
		Valid: 2,
		InputEmpty: 3
	},

	//REGISTER
	registerCallbackOperation: {
		Error: 0,
		Exists: 1,
		Valid: 2,
		UsernameTooShort: 3,
		UsernameBadChars: 4,
		PasswordTooShort: 5,
		EmailUsed: 6,
		EmailInvalid: 7
	}
};