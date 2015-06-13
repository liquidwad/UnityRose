using UnityEngine;
using System.Collections;

public enum Job1Type {
	VISITOR,
	HAWKER,
	SOLDIER,
	MUSE,
	DEALER,
	numJobTypes
}

public enum Job2Type {
	NONE,
	SCOUT,
	RAIDER,
	KNIGHT,
	CHAMPION,
	MAGE,
	CLERIC,
	ARTISAN,
	BOURGEOIS,
	numJobTypes	
}

public enum GenderType {
	MALE,
	FEMALE,
	numGenders
}

public class Stats {

	public float atk { get; set; }
	public float def { get; set; }
	public float dex { get; set; }
	public float intel { get; set; } 
	public float crit { get; set; }
	public float luck { get; set; }
	public float movSpd { get; set; }
	public float atkSpd { get; set; }
}

public class Equip {
	public int faceID { get; set; }
	public int hairID { get; set; }
	public int chestID { get; set; }
	public int footID { get; set; }
	public int handID { get; set; }
	public int weaponID { get; set; }
	public int shieldID { get; set; }
	public int backID { get; set; }
}

public class CharModel {
	public Job1Type job0 { get; set; }
	public Job2Type job1 { get; set; }
	public int level { get; set; }
	public Vector3 pos { get; set; }
	public Stats stats { get; set; }
	public GenderType gender { get; set; }
	public Equip equip { get; set; }
}
