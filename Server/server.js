'use strict';

var net = require('net'),
	JsonSocket = require('json-socket'),
	consoleStamp = require('console-stamp'),
	config = require('./config'),
	common = require('./common'),
	crypto = require('./crypto');


consoleStamp(console, "dd mmm HH:mm:ss -");

//LOAD MODELS
//var mongoose = require('./models');

var server = net.createServer();

server.on('connection', function(socket) {
	//handle client
	console.log("Client connected from " + socket.remoteAddress);

	var encrypted = crypto.encrypt({ id: 133232, posX: 100, posY: 200, mapId: 44 });

	socket.write(encrypted.toString());

	socket.on('data', function(message) {
		console.log("Received: " + message);
		var decrypted = crypto.decrypt( message );
		console.log("Decrypted: " + JSON.stringify(decrypted));
	});

	socket.on('end', function() {
		console.log("client disconnected");
	});

	socket.on('error', function(e) {
		console.log("Exception: " + e.errno);
	});
});

server.listen(config.PORT, function() {
	console.log("Server listening on port " + server.address().port);
});