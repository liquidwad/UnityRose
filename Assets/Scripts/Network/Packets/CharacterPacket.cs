using UnityEngine;
using System.Collections;
using JsonFx.Json;


namespace Network.Packets
{
	public enum CharacterOperation
	{
		GROUNDCLICK = 1,
		CHANGEDSTATE = 2,
		INSTANTIATE = 3
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
			return output.ToString();
		}
	}
	
	[JsonOptIn]
	public class InstantiateChar: CharacterPacket
	{
		[JsonMember]
		public Vector3 position {get; set;}
		
		[JsonMember]
		public Quaternion rotation {get; set;}
		
		
		//Todo: add members for armor, speed, etc
		
		public InstantiateChar()
		{
		}
		
		public InstantiateChar(string clientID, Vector3 position, Quaternion rotation)
		{
			this.clientID = clientID;
			this.position = position;
			this.rotation = rotation;
			
			operation = (int)CharacterOperation.INSTANTIATE;
		}
		
		public override string toString()
		{
			writer.Write (this);
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
			return output.ToString();
		}
	}
}

