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
		Valid: 2
	},

	//REGISTER
	registerCallbackOperation: {
		Error: 0,
		Exists: 1,
		Valid: 2
	}
};