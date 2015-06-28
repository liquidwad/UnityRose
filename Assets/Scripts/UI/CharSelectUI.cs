using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace UnityRose
{
	public class CharSelectUI : MonoBehaviour
	{
		public Transform[] playerPositions;
		public Dictionary<int,RosePlayer> players;
        RosePlayer currentPlayer;

		// Use this for initialization
		void Start () {
			players = new Dictionary<int, RosePlayer> ();
			// TODO: get packets from server to populate players
			// For now simulate loading 3 players
			onLoad ( new CharModel("Hadak", GenderType.MALE) );
			CharModel hawker = new CharModel("Ferkh", GenderType.FEMALE);
			hawker.equip = new Equip(97,97,97,97,0,0,1,0,0,0);
			onLoad ( hawker );
			CharModel knight = new CharModel("3awd", GenderType.MALE);
			knight.equip = new Equip(42,42,42,42,0,0,1,0,0,0);
			onLoad ( knight );
			onLoad ( new CharModel("Hadik", GenderType.FEMALE ));
				
			
		}

		public void onLoad(CharModel charModel) {
			// Do nothing if we already reached the max number of chars
			if (players.Count >= 5)
				return;
				
			// Generate a default player
			charModel.rig = RigType.CHARSELECT;
			charModel.state = States.SITTING;
			RosePlayer player = new RosePlayer (charModel);
			int id = players.Count;
			Vector3 altarPos = playerPositions [id].position;
			player.player.transform.position = new Vector3 (altarPos.x, altarPos.y + 2.1f, altarPos.z);
			player.player.transform.LookAt (Camera.main.transform);
			players.Add ( id, player );
			currentPlayer = player;
			
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
			if(currentPlayer != null)
				currentPlayer.setAnimationState(States.SIT);
            currentPlayer = player;

		}



        void OnPlayerDoubleClick(int id)
        {
            players[id].setAnimationState(States.SELECT);
        }

        // Update is called once per frame
        void Update () {

            bool locate = false;
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                case RuntimePlatform.WP8Player:
                    locate = Input.touchCount > 0;
                    break;
                default:
                    locate = Input.GetMouseButton(0);
                    break;

            }

            if (locate)
                LocatePlayer();
        }

        void LocatePlayer()
        {
            Debug.Log("LocatePlayer()");
            Vector2 screenPoint;
            bool fire = false;
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                case RuntimePlatform.WP8Player:
                    screenPoint = Input.GetTouch(0).position;
                    fire = (Input.GetTouch(0).tapCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Ended);
                    break;
                default:
                    screenPoint = Input.mousePosition;
                    fire = Input.GetMouseButtonDown(0);
                    break;

            }

            Ray camRay = Camera.main.ScreenPointToRay(screenPoint);
            RaycastHit playerHit;

            if (fire)
            {
                // Perform the raycast and if it hits something on the floor layer...
                if (Physics.Raycast(camRay, out playerHit, 500.0f))//, LayerMask.NameToLayer("Players")))
                    OnPointerClick(playerHit.transform);
            }
        }


        void OnPointerClick(Transform clickedTransform)
        {
            Debug.Log("OnPointerCLick");
            PlayerController controller = clickedTransform.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                RosePlayer clickedPlayer = controller.rosePlayer;
                if (clickedPlayer != currentPlayer)
                {
                    clickedPlayer.setAnimationState(States.STANDUP);
                    currentPlayer.setAnimationState(States.SIT);
                    currentPlayer = clickedPlayer;
                }
            }
        }
    }
}
