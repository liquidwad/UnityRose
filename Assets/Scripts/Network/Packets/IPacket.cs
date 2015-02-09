using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonFx.Json;
using UnityEngine;
using Network.JsonConverters;

namespace Network.Packets
{
	public enum PacketType
	{
		USER=1,
		CHARACTER=2
	}
	
	[JsonOptIn]
    public class Packet
    {
		[JsonMember]
		public int type { get; set; }
		[JsonMember]
    	public int operation { get; set; }
    	
    	protected JsonWriter writer;
    	protected StringBuilder output;
    	
    	public Packet()
    	{
    		type = 0;
    		operation = 0;
    		
			output = new StringBuilder();
			JsonWriterSettings settings = new JsonWriterSettings();
			settings.PrettyPrint = false;
			settings.AddTypeConverter (new VectorConverter());
			// TODO: Add any other TypeConverters here
			writer = new JsonWriter (output,settings);
    	}
    	
        public virtual string toString()
        {
			writer.Write (this);
			return output.ToString();
		}
	}
	
}
