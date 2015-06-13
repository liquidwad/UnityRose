'use strict';

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var userSchema = new Schema({
	_userID: Schema.Types.ObjectId,
	username: String,
	password: String,
	email: String,
	server: Number,
	numChars: { type: Number, default: 0 },
	_chars: [Schema.Types.ObjectId],
	_activeChar: Schema.Types.ObjectId
});

var User = mongoose.model('User', userSchema);

module.exports = User;