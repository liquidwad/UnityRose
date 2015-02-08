using System;
using Network;

namespace UnityRoseClient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			NetworkManager.Start ();

			NetworkManager.Send (new Network.Packets.Character.GroundClick ("Wahid", new Network.Packets.Character.Vector3(105.4f, 99.7f, 42.6f)));

			Console.ReadKey (false);
		}
	}
}
