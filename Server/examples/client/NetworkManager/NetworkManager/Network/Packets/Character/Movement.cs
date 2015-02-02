using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonFx.Json;

namespace NetworkManager.Network.Packets.Character
{
    public class MovementArgs
    {

    }

    class Movement : IPacket
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
            JsonWriter writer = new JsonWriter();
            
            string json = writer.Write(new {
                clientid=ClientID,
                x=X,
                y=Y
            });

            return json;
        }
    }
}
