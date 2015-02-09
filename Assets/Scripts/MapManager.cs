using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Network;
using Network.Packets;
using UnityRose;

public class MapManager : MonoBehaviour {

	public string mapID;
	public string mainChar;
	public GameObject playerFab;
	
	private List<GameObject> players;
	private int numPlayers;
	
	private Queue<Packet> packetQueue;
	
	// Use this for initialization
	void Start () {
	
		players = new List<GameObject>();
		packetQueue = new Queue<Packet>();
		numPlayers = 0;
		
		// Add definitions for all packet received delegates
		NetworkManager.instantiateCharDelegate += (InstantiateChar packet) => 
		{
			packetQueue.Enqueue( packet );
			numPlayers++;
		};
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if( packetQueue.Count > 0)
		{
			Packet packet = packetQueue.Dequeue();
			
			switch( (PacketType)packet.type )
			{
				case PacketType.CHARACTER:
					switch( (CharacterOperation)packet.operation )
					{
						case CharacterOperation.INSTANTIATE:
							InstantiateChar ic = (InstantiateChar) packet;
							if( ic.clientID != mainChar )
							{
								GameObject newPlayer = (GameObject)Instantiate( playerFab, ic.position, ic.rotation);
								
								// TODO: add all other player initialization specifications here based on packet
								newPlayer.name = ic.clientID; // + " " + numPlayers;
								
								PlayerController playerController = newPlayer.GetComponent<PlayerController>();
								playerController.isMainPlayer = false;
								playerController.name = newPlayer.name;
								players.Add(newPlayer);
							}
							break;
						default:
							break;
					}
					break;
				default:
					break;
			}
		}
	}
}
