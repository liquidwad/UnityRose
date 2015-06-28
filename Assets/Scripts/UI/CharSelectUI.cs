using UnityEngine;
using System.Collections.Generic;

namespace UnityRose
{
	public class CharSelectUI : MonoBehaviour 
	{
		public Transform[] playerPositions;
		public Dictionary<int,RosePlayer> players;
        int currentPlayer;

		// Use this for initialization
		void Start () {
			players = new Dictionary<int, RosePlayer> ();
			// TODO: get packets from server to populate players
		}

		public void onCreate() { 
			// Do nothing if we already reached the max number of chars
			if (players.Count >= 5)
				return;

			// Generate a default player
			CharModel charModel = new CharModel ();
            charModel.rig = RigType.CHARSELECT;
            charModel.state = States.HOVERING;
            RosePlayer player = new RosePlayer (charModel);
            int id = players.Count;
			Vector3 altarPos = playerPositions [id].position;
			player.player.transform.position = new Vector3 (altarPos.x, altarPos.y + 2.1f, altarPos.z);
			player.player.transform.LookAt (Camera.main.transform);
			players.Add ( id, player );
            currentPlayer = id;

		}

        void OnPlayerClick(int id)
        {
            if (id != currentPlayer)
            {
                players[id].setAnimationState(States.STANDUP);
                players[currentPlayer].setAnimationState(States.SIT);
            }
            
        }

        void OnPlayerDoubleClick(int id)
        {
            players[id].setAnimationState(States.SELECT);
        }

        // Update is called once per frame
        void Update () {
		
		}
	}
}
