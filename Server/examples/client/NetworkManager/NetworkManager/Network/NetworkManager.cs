using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace NetworkManager.Network
{
    public class State {
        public Socket socket;

        public const int BufferSize = 256;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();
    }

    public class NetworkManager
    {
        private const string ipAddress = "127.0.0.1";

        private const int port = 3000;

        private static Socket socket = null;

        public static void StartClient()
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);

                IPEndPoint remoteEp = new IPEndPoint(ip, port);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket = client;

                client.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                Recieve();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
                Console.WriteLine(e.ToString());
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
                        Console.WriteLine("Recieved {0}", state.sb.ToString());
                    }

                    Crypto.Decrypt(state.sb.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Send(Packets.IPacket packet)
        {
            if(socket == null) {
                return;
            }

            byte[] byteData = Encoding.ASCII.GetBytes(packet.ToString());

            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), socket);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSend = client.EndSend(ar);

                Console.WriteLine("Send {0} bytes to server.", bytesSend);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
