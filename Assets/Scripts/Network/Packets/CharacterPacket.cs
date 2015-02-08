using UnityEngine;
using System.Collections;
using JsonFx.Json;


namespace Network.Packets
{
	public enum CharacterOperation
	{
		GROUNDCLICK = 1,
		CHANGEDSTATE = 2
	}
	
	[JsonOptIn]
	public class CharacterPacket: Packet
	{
		[JsonMember]
		public string clientID {get; set;}
		
		public CharacterPacket()
		{
			type = (int)PacketType.CHARACTER;
			operation = 0;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			Debug.Log(output.ToString());
			return output.ToString();
		}
	}
	
	[JsonOptIn]
	public class GroundClick: CharacterPacket
	{
		[JsonMember]
		public Vector3 pos {get; set;}
		
		public GroundClick()
		{
		}
		
		public GroundClick(string clientID, Vector3 position)
		{
			this.clientID = clientID;
			this.pos = position;
			
			operation = (int)CharacterOperation.GROUNDCLICK;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			Debug.Log(output.ToString());
			return output.ToString();
		}
	}
	
	[JsonOptIn]
	public class ChangedState: CharacterPacket
	{
		[JsonMember]
		public string state {get; set;}
		
		public ChangedState(string clientID, string state)
		{
			this.clientID = clientID;
			this.state = state;
			operation = (int)CharacterOperation.CHANGEDSTATE;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			Debug.Log(output.ToString());
			return output.ToString();
		}
	}
}

