'use strict';

var CryptoJS = require('node-cryptojs-aes').CryptoJS,
	JsonFormatter = require('node-cryptojs-aes').JsonFormatter,
	crypto = require('crypto'),
	config = require('./config');

module.exports.encrypt = function(data) {

	var message = data;

	if(Object.prototype.toString.call(data) == "[object Object]") {
		try {
			message = JSON.stringify(data);
		}
		catch(e) {
			console.log(e);
		}
	}

	return CryptoJS.AES.encrypt(message, config.secret_phrase, { format: JsonFormatter });
};

module.exports.decrypt = function(data) {

	var data = data;

	if(Object.prototype.toString.call(data) == "[object Object]") {
		try {
			data = JSON.stringify(data);
		}
		catch(e) {
			console.log(e);
		}
	}
	
	var decrypted = CryptoJS.AES.decrypt(data, config.secret_phrase, { 
		format: JsonFormatter
	});

	return CryptoJS.enc.Utf8.stringify(decrypted);
};