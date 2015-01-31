'use strict';

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

module.exports = function() {

	var userSchema = new Schema({
		name: String
	});

	var User = mongoose.model('User', userSchema);

	//return { 'User': User };
};