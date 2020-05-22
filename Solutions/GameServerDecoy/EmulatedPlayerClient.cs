using Newtonsoft.Json;
using ServerManager;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GameServerDecoy {
    class EmulatedPlayerClient {

        private int _bufferSize = 4096;
        private byte [ ] _buffer;
        public TcpClient TcpClient;
        Packet incomingPacket = new Packet();

        public EmulatedPlayerClient (TcpClient client) {
            _buffer = new byte [ _bufferSize ];
            TcpClient = client;

            TcpClient.ReceiveBufferSize = _bufferSize;
            TcpClient.GetStream().BeginRead(_buffer, 0, _bufferSize, TcpCallback, null);
        }

        public void TcpCallback (IAsyncResult asyncResult) {
            try {


                int messageLength = TcpClient.GetStream().EndRead(asyncResult);
                //Check if we received more than 0 bytes
                if (messageLength <= 0)
                {
                    return;
                }

                byte[] messageData = new byte[messageLength];

                Array.Copy(_buffer, messageData, messageLength);

                incomingPacket.Reset(HandleIncomingData(messageData));

                TcpClient.GetStream().BeginRead(_buffer, 0, _bufferSize, TcpCallback, null);               

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private bool HandleIncomingData(byte[] data)
        {
            //Read the length of the next data packet
            int packetLength = 0;

            //Check if there is 4 or more bytes in the packet (size of int is 4)
            if (incomingPacket.UnreadLength() >= 4)
            {
                //Read the length of the incoming packet
                packetLength = incomingPacket.ReadInt();
                if (packetLength <= 0)
                {
                    //If the length is 0, return
                    return true;
                }
            }

            //Keep reading until there is no more data left to read for this specific packet
            //We keep this in a while loop here because one packet might be made up of several packets
            while (packetLength > 0 && packetLength <= incomingPacket.UnreadLength())
            {
                byte[] bytes = incomingPacket.ReadBytes(packetLength);

                //Handle packet
                //Read the messagetype
                int messageTypeInt = incomingPacket.ReadInt();
                MessageType messageType = (MessageType)messageTypeInt;
                string messageJSON = "";

                switch (messageType)
                {
                    case MessageType.Ready:
                        messageJSON = incomingPacket.ReadString();
                        GameserverInstance gameserverReady = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                        EmulatedUserConnection.ReceiveServerInfo(gameserverReady);
                        break;
                    default:
                        break;
                }

                packetLength = 0;

                //Check if there 4 or more bytes in the packet (size of int is 4)
                if (incomingPacket.UnreadLength() >= 4)
                {
                    //Read the length of the incoming packet
                    packetLength = incomingPacket.ReadInt();
                    if (packetLength <= 0)
                    {
                        //If the length is 0, return
                        return true;
                    }

                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

    }
}
