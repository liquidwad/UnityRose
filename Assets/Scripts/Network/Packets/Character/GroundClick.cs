using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using JsonFx.Json;
using UnityEngine;
using Network.JsonConverters;

namespace Network.Packets.Character
{
    class GroundClick : IPacket
    {
		[JsonMember]
        private string clientID;
		[JsonMember]
        private Vector3 pos;

        public GroundClick(string clientId, Vector3 pos)
        {
           	this.clientID = clientId;
            this.pos = pos;
        }

        public new String ToString()
        {
			StringBuilder output = new StringBuilder();
			JsonWriterSettings settings = new JsonWriterSettings();
			settings.PrettyPrint = false;
			settings.AddTypeConverter (new VectorConverter());
			JsonWriter writer = new JsonWriter (output,settings);
			writer.Write (this);			
								     
            Debug.Log(output.ToString());
                  
          	return output.ToString();
        }
    }
}
