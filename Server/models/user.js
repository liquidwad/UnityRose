'use strict';

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var userSchema = new Schema({
	username: String,
	password: String,
	email: String,
	server: Number,
	online: Boolean,
	_chars: [String],
	_activeChar: String
});

var User = mongoose.model('User', userSchema);

module.exports = User;