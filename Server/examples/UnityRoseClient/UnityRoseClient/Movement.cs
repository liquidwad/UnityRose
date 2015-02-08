using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using JsonFx.Json;

namespace Network.Packets.Character
{
    public class MovementArgs
    {

    }

    class Movement
    {
        public string ClientID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Movement(string clientId, int x, int y)
        {
            this.ClientID = clientId;
            this.X = x;
            this.Y = y;
        }

        public new String ToString()
        {
        	StringBuilder output = new StringBuilder();
            JsonWriter writer = new JsonWriter( output );
            
            writer.Write(new {
                clientid=ClientID,
                x=X,
                y=Y
            });

            return output.ToString();
        }
    }
}
