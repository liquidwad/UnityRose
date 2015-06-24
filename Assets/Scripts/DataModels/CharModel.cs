using UnityEngine;
using System.Collections;

namespace UnityRose
{
	public class Stats {

		public float atk { get; set; }
		public float def { get; set; }
		public float dex { get; set; }
		public float intel { get; set; } 
		public float crit { get; set; }
		public float luck { get; set; }
		public float movSpd { get; set; }
		public float atkSpd { get; set; }

		public Stats()
		{
			atk = 10;
			def = 10;
			dex = 10;
			intel = 10;
			crit = 10;
			luck = 10;
			movSpd = 10;
			atkSpd = 10;
		}
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
		public int maskID { get; set; }
		public int capID { get; set; }

		public Equip(){	
			faceID = 1;
			hairID = 0;
			chestID = 0;
			footID = 0;
			handID = 0;
			weaponID = 1;
			shieldID = 0;
			backID = 0;
			maskID = 0;
			capID = 0;
		}
	}

	public class CharModel {
		public string name { get; set; }
		public Job1Type job0 { get; set; }
		public Job2Type job1 { get; set; }
		public int level { get; set; }
		public Vector3 pos { get; set; }
		public Stats stats { get; set; }
		public GenderType gender { get; set; }
		public Equip equip { get; set; }

		public CharModel()
		{
			name = "New";
			gender = GenderType.MALE;
			job0 = Job1Type.VISITOR;
			job1 = Job2Type.NONE;
			level = 1;
			pos = new Vector3 (0, 0, 0);
			stats = new Stats ();
			equip = new Equip ();
		}
	}
}
