using UnityEngine;
using System.Collections.Generic;

namespace UnityRose
{
	public class CharSelectUI : MonoBehaviour 
	{
		public Transform[] playerPositions;
		public List<RosePlayer> players;
		
		// Use this for initialization
		void Start () {
			players = new List<RosePlayer> ();
			// TODO: get packets from server to populate players
		}

		public void onCreate() { 
			// Do nothing if we already reached the max number of chars
			if (players.Count >= 5)
				return;

			// Generate a default player
			CharModel charModel = new CharModel ();
			RosePlayer player = new RosePlayer (charModel);
			Vector3 altarPos = playerPositions [players.Count].position;
			player.player.transform.position = new Vector3 (altarPos.x, altarPos.y + 2.1f, altarPos.z);
			player.player.transform.LookAt (Camera.main.transform);
			players.Add ( player );

		}

		// Update is called once per frame
		void Update () {
		
		}
	}
}
