using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Common.Cryptography;

namespace Network
{
    public class State {
        public Socket socket;

        public const int BufferSize = 256;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();
    }

    public class NetworkManager : MonoBehaviour
    {
		//NetworkManager.Network.NetworkManager.StartClient();
		private const string ipAddress = "50.139.144.237";

        private const int port = 3000;

        private static Socket socket = null;
        
		private static AES crypto;
			
		void Start()
        {
            try
            {
            	crypto = new AES();
                IPAddress ip = IPAddress.Parse(ipAddress);

                IPEndPoint remoteEp = new IPEndPoint(ip, port);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket = client;

                client.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), client);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());

                Recieve();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        /* 
         * var packet = Recieve();
         * 
         * switch(packet.opcode) {
         *  sendPacket();
         * }
         */
        public static void Recieve()
        {
            if (socket == null)
            {
                return;
            }

            try
            {
                State state = new State();
                state.socket = socket;
                socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        private static void RecieveCallback(IAsyncResult ar)
        {
            try
            {
                State state = (State)ar.AsyncState;
                Socket client = state.socket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    if (state.sb.Length > 1)
                    {
                        Debug.Log("Recieved " + state.sb.ToString());
                    }

                    string clearText = crypto.Decrypt(state.sb.ToString());
                    Debug.Log ("Decrypted: " + clearText);
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public static void Send(Packets.IPacket packet)
        {
            if(socket == null) {
                return;
            }

            byte[] byteData = Encoding.ASCII.GetBytes( crypto.Encrypt(packet.ToString()));

            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSend = client.EndSend(ar);

				Debug.Log("Send " + bytesSend + " bytes to server.");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}
