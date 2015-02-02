using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkManager
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkManager.Network.NetworkManager.StartClient();

            Console.ReadKey(false);
        }
    }
}
