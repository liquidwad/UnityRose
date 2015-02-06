'use strict';

var net = require('net'),
	JsonSocket = require('json-socket'),
	consoleStamp = require('console-stamp'),
	config = require('./config'),
	common = require('./common'),
	packet = require('./packet'),
	crypto = require('./crypto'),
	World = require('./world/world');


consoleStamp(console, "dd mmm HH:mm:ss");

//load models and database
//var mongoose = require('./models');



//Create server
var server = net.createServer();

//Create new world
var world = new World();

server.on('connection', function(socket) {

	console.log("Client connected from " + socket.remoteAddress);

	socket.on('data', function(data) {
		var data = crypto.decrypt( data ),
			type = packet.packetType.get( data.type );

		console.log("Packet type: " + type);

		switch(data.type) {
			case packet.packetType.User.value:
				console.log("Handle User packet");
				break;
			case packet.packetType.Character.value:
				console.log("Handle character packet");
				break;
			default:
				console.log("unknown packet");
				break;
		}

		console.log( JSON.stringify( data ) );
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