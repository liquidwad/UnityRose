using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonFx.Json;
using System.IO;

namespace Network.Packets.Character
{

	public class Vector3 {

		private float x;

		private float y;

		private float z;

		public Vector3(float x, float y, float z) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

    public class GroundClick : IPacket
    {
		private int type;

		private int operation;

        private string clientID;

		private Vector3 position;

		public GroundClick(string clientId, Vector3 position)
        {
			this.type = (int)UnityRoseClient.Types.CHARACTER;
			this.operation = (int)UnityRoseClient.CharacterOperations.WALK;
           	this.clientID = clientId;
			this.position = position;
		}

        public new String ToString()
        {
			StringBuilder output = new StringBuilder();

			JsonWriter writer = new JsonWriter (output);
			writer.Write (this);			

			Console.WriteLine (output.ToString ());
                  
          	return output.ToString();
        }
    }
}
