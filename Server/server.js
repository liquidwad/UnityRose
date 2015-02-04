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

/*var encrypted = crypto.encrypt({ id: 133232, posX: 100, posY: 200, mapId: 44 });

var f = encrypted.toString();

console.log(f);

console.log( crypto.decrypt( JSON.parse(f) ) );*/

var server = net.createServer();

server.on('connection', function(socket) {
	//handle client
	console.log("Client connected from " + socket.remoteAddress);

	var encrypted = crypto.encrypt({ id: 133232, posX: 100, posY: 200, mapId: 44 });

	socket.write(encrypted.toString());

	socket.on('message', function(message) {
		console.log(message);
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