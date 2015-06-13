'use strict';

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var charSchema = new Schema({
	_charID: Schema.Types.ObjectId,
	_user: Schema.Types.ObjectId,
	_map: Number,
	_spawn: Number,
	_party: Schema.Types.ObjectId,
	_guild: Schema.Types.ObjectId,
	_job: Number,
	// TODO: remove the nonID versions when the other db's are filled
	party: String,
	guild: String,
	level: { type: Number, default: 1 },
	job0: { type: String, default: "Visitor" },
	job1: String,
	pos: { x: Number, y: Number, z: Number},
	stats: {
		atk: { type: Number, default: 10 },
		def: { type: Number, default: 10 },
		dex: { type: Number, default: 10 },
		int: { type: Number, default: 10 },
		crit: { type: Number, default: 10 },
		luck: { type: Number, default: 10 },
		movSpd: { type: Number, default: 10 },
		atkSpd: { type: Number, default: 10 }
	},
	gender: String,
	equip: {
		faceID: Number,
		hairID: Number,
		chestID: Number,
		legID: Number,
		shoeID: Number,
		handID: Number,
		weaponID: Number,
		shieldID: Number,
		backID: Number
	}
		
});

var Char = mongoose.model('Char', charSchema);

module.exports = Char;