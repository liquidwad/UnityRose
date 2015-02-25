using UnityEngine;
using System.Collections;
using JsonFx.Json;


namespace Network.Packets
{
	public enum UserOperation
	{
		REGISTER = 0,
		LOGIN = 1,
		CHARSELECT = 2
	}
	
	public enum LoginStatus
	{
		ERROR = 0,
		NOT_EXIST = 1,
		VALID = 2,
		INPUT_EMPTY = 3
	}
	
	public enum RegisterResponse
	{
		ERROR = 0,
		USERINVALID_EXISTS = 1,
		SUCCESS = 2,
		USERINVALID_TOOSHORT = 3,
		USERINVALID_BADCHARS = 4,
		PASSWORD_TOOSHORT = 5,
		EMAIL_USED = 6,
		EMAIL_INVALID = 7
	}
	
	[JsonOptIn]
	public class RegisterPacket: Packet 
	{
		[JsonMember]
		public string username { get; set; }
		
		[JsonMember]
		public string password { get; set; }
		
		[JsonMember]
		public string email { get; set; }
		
		public RegisterPacket()
		{
		}
		
		public RegisterPacket(string username, string email, string password)
		{
			this.username = username;
			this.password = password;
			this.email = email;
			operation = (int)UserOperation.REGISTER;
			type = (int)PacketType.USER;
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
	public class LoginPacket: Packet
	{
		[JsonMember]
		public string username { get; set; }
		
		[JsonMember]
		public string password {get; set; }
		
		public LoginPacket()
		{
		
		}
		
		public LoginPacket(string username, string password)
		{
			this.username = username;
			this.password = password;
			operation = (int)UserOperation.LOGIN;
			type = (int)PacketType.USER;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
	
	// This packet is sent by the client after selecting a character in the char select scene
	[JsonOptIn]
	public class CharSelectPacket: Packet
	{
		[JsonMember]
		public string characterID {get; set; }
		
		public CharSelectPacket()
		{
		}
		
		public CharSelectPacket(string characterID)
		{
			this.characterID = characterID;
			type = (int)PacketType.USER;
			operation = (int)UserOperation.CHARSELECT;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
	
	///////////////// Server -> Client packets //////////////////////
	[JsonOptIn]
	public class LoginReply: Packet
	{
		[JsonMember]
		public int response { get; set; }
		
		public LoginReply()
		{
			type = (int)PacketType.USER;
			operation = (int)UserOperation.LOGIN;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
	
	[JsonOptIn]
	public class RegisterReply: Packet
	{
		[JsonMember]
		public int response { get; set; }
		
		public RegisterReply()
		{
			type = (int)PacketType.USER;
			operation = (int)UserOperation.REGISTER;
		}
		
		public override string toString()
		{
			writer.Write (this);			
			return output.ToString();
		}
	}
}

