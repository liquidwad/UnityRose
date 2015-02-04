'use strict';

var CryptoJS = require("crypto-js"), 
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

	console.log("Encrypting message: " + message);

	var encrypted = CryptoJS.AES.encrypt(message,
		CryptoJS.enc.Utf8.parse(config.key), { 
		mode: CryptoJS.mode.CBC, 
		padding: CryptoJS.pad.Pkcs7,
		iv: CryptoJS.enc.Utf8.parse(config.iv) 
	});

	return encrypted;
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
	
	var decrypted = CryptoJS.AES.decrypt(data, CryptoJS.enc.Utf8.parse(config.key), { 
		mode: CryptoJS.mode.CBC, 
		padding: CryptoJS.pad.NoPadding,
		iv: CryptoJS.enc.Utf8.parse(config.iv) 
	});


	return CryptoJS.enc.Utf8.stringify(decrypted);
};