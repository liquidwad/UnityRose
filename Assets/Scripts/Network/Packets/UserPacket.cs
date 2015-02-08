using UnityEngine;
using System.Collections;
using JsonFx.Json;


namespace Network.Packets
{
	public enum UserOperation
	{
		LOGIN = 1
	}
	
	[JsonOptIn]
	public class UserPacket: Packet
	{
		public UserPacket()
		{
			type = (int)PacketType.USER;
			operation = 0;
		}
	}
	
	[JsonOptIn]
	public class Login: UserPacket
	{
		[JsonMember]
		public string username { get; set; }
		
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
	}
	
}

