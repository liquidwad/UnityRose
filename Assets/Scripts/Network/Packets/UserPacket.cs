using UnityEngine;
using System.Collections;
using JsonFx.Json;


namespace Network.Packets
{
	public enum UserOperation
	{
		DEFAULT = 0,
		LOGIN = 1,
		CHARSELECT = 2
	}
	
	[JsonOptIn]
	public class UserPacket: Packet
	{
		[JsonMember]
		public string username { get; set; }
		
		public UserPacket()
		{
			type = (int)PacketType.USER;
			operation = (int)UserOperation.DEFAULT;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
	
	///////////////// Client -> Server packets //////////////////////
	
	// This packet is used by the user to login to the char select scene
	[JsonOptIn]
	public class Login: UserPacket
	{
		[JsonMember]
		public string password {get; set; }
		
		public Login()
		{
		}
		
		public Login(string username, string password)
		{
			this.username = username;
			this.password = password;
			operation = (int)UserOperation.LOGIN;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
	
	// This packet is sent by the client after selecting a character in the char select scene
	[JsonOptIn]
	public class CharSelect: UserPacket
	{
		[JsonMember]
		public string username { get; set; }
		
		[JsonMember]
		public string characterID {get; set; }
		
		public CharSelect()
		{
		}
		
		public CharSelect(string username, string characterID)
		{
			this.username = username;
			this.characterID = characterID;
			operation = (int)UserOperation.CHARSELECT;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
	
	///////////////// Server -> Client packets //////////////////////
	
	
	
}

