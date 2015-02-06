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
		
	var decrypted = CryptoJS.AES.decrypt(data.toString(), 
		CryptoJS.enc.Utf8.parse(config.key), { 
		mode: CryptoJS.mode.CBC, 
		padding: CryptoJS.pad.Pkcs7,
		iv: CryptoJS.enc.Utf8.parse(config.iv) 
	});
	
	var packet = {};
	
	try {
		var decryptedText = decrypted.toString(CryptoJS.enc.Utf8); // CryptoJS.enc.Utf8.parse( decrypted.toString() );
		
		console.log( decryptedText );
		packet = JSON.parse( decryptedText );
	}
	catch(e) {
		console.log(e);
	}
	
	//var clearText = decrypted.toString(CryptoJS.enc.Utf8);
	
	return packet;
};