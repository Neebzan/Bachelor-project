using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class ServerClient
{
    public static int dataBufferSize = 4096;

    public int id;
    public Player player;

    public TCP tcp;

    public ServerClient(int _id)
    {
        id = _id;
        tcp = new TCP(id);
    }

    public class TCP
    {
        public TcpClient client;
        private readonly int id;

        private NetworkStream stream;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _client)
        {
            client = _client;
            client.ReceiveBufferSize = dataBufferSize;

            stream = client.GetStream();

            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerPacketSender.WelcomeMessage(id, "This is a welcome message!");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if(client != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Couldn't sent data to {id} via TCP: {e}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int bytesReceived = stream.EndRead(_result);
                //
                if(bytesReceived <= 0)
                {
                    Server.clients[id].tcp.Disconnect();
                    return;
                }

                byte[] data = new byte[bytesReceived];
                Array.Copy(receiveBuffer, data, bytesReceived);

                //Wait for next data
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        /// <summary>Closes and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            client.Close();
            stream = null;
            //receivedData = null;
            receiveBuffer = null;
            client = null;
        }
    }

}
