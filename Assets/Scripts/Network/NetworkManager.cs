// <copyright file="NetworkManager.cs" company="Wadii Bellamine">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author> Wadii Bellamine, Wahid Bouakline</author>
// <date>2/25/2015 8:37 AM </date>

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
using Network.Packets;
using Network.JsonConverters;
using JsonFx.Json;
using System.Runtime.CompilerServices;

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
		private const string ipAddress = "24.62.143.241"; 

        private const int port = 3000;

        private static Socket socket = null;
        
		private static AES crypto;
		
		//public delegate void Reply(Packet packet);
		
		void Awake()
        {
            Connect();
        }
        
        private static void Connect() 
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

        public static void Recieve()
        {
			if (socket == null || !socket.Connected)
            {
            	Connect();
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
                    
                    string data = crypto.Decrypt(state.sb.ToString());
                    Debug.Log (data);

                    JsonReader reader = new JsonReader(data);
              		
              		Packet packet = reader.Deserialize<Packet>();
              		
              		PacketType type = (PacketType)packet.type;
              		
              		switch(type) {
              			case PacketType.CHARACTER:
              				CharacterManager.Instance.handlePacket(packet.operation, data);
              				break;
              			case PacketType.USER:
              				UserManager.Instance.handlePacket(packet.operation, data);
              				break;
					default: 
						break;
					
              		}					
                }

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            
            Recieve();
        }

		
        public static void Send(Packets.Packet packet)
        {
			if (socket == null || !socket.Connected)
			{
				Connect();
				return;
			}

			string clearText = packet.toString();
			Debug.Log("Sending: " + clearText);
			byte[] byteData = Encoding.ASCII.GetBytes( crypto.Encrypt(clearText) );

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
