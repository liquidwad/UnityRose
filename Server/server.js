'use strict';

var _ = require('lodash'),
	net = require('net'),
	JsonSocket = require('json-socket'),
	consoleStamp = require('console-stamp'),
	shortid = require('shortid'),
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

var clients = [];

server.on('connection', function(socket) {

	console.log("Client connected from " + socket.remoteAddress);

	socket.id = shortid.generate();

	clients.push(socket);

	socket.on('data', function(data) {
		var data = crypto.decrypt( data ),
			type = packet.packetType.get( data.type );

		switch(data.type) {
			case packet.packetType.User.value:
				console.log("Handle User packet");
				break;
			case packet.packetType.Character.value:
				console.log("Handle character packet");

				var clientMoveData = crypto.encrypt(data);

				_.each(clients, function(client) {
					/*if(socket === client) {
						return;
					}*/

					client.write(clientMoveData);
				});
				break;
			default:
				console.log("unknown packet");
				break;
		}

		console.log( "[" + socket.id + "][" + type + "] - " + JSON.stringify( data ) );
	});

	socket.on('end', function() {
		console.log("client disconnected");
	});

	socket.on('error', function(e) {
		console.log("Exception: " + JSON.stringify(e));
	});
});

server.listen(config.PORT, function() {
	console.log("Server listening on port " + server.address().port);
});