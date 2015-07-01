'use strict';

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var userSchema = new Schema({
	username: String,
	password: String,
	email: String,
	server: Number,
	online: Boolean,
	_chars: [Schema.Types.ObjectId],
	_activeChar: Schema.Types.ObjectId
});

var User = mongoose.model('User', userSchema);

module.exports = User;