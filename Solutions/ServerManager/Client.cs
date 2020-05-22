using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ServerManager
{
    public class Client
    {
        private int _bufferSize = 4096;
        private byte[] _buffer;
        public TcpClient TcpClient;

        byte[] incomingData;
        List<byte> incomingBytes = new List<byte>();
        int incomingPacketSize = -1;
        int readIndex = 0;
        bool sizeSet = false;

        public Client(TcpClient client)
        {
            _buffer = new byte[_bufferSize];
            TcpClient = client;

            TcpClient.ReceiveBufferSize = _bufferSize;
            TcpClient.GetStream().BeginRead(_buffer, 0, _bufferSize, TcpCallback, null);
        }

        public void TcpCallback(IAsyncResult asyncResult)
        {
            try
            {
                int messageLength = TcpClient.GetStream().EndRead(asyncResult);


                //Check if we received more than 0 bytes
                if (messageLength <= 0)
                {
                    return;
                }

                byte[] messageData = new byte[messageLength];

                Array.Copy(_buffer, 0, messageData, 0, messageLength);

                if(HandleIncomingData(messageData))
                {
                    incomingBytes = new List<byte>();
                    incomingPacketSize = -1;
                    readIndex = 0;
                    sizeSet = false;
                }

                TcpClient.GetStream().BeginRead(_buffer, 0, _bufferSize, TcpCallback, null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool HandleIncomingData(byte[] data)
        {
            //Read the length of the next data packet
            if (incomingPacketSize < 0)
            {
                if (data.Length + incomingBytes.Count >= 4)
                {
                    incomingPacketSize = BitConverter.ToInt32(data, readIndex);
                    readIndex += 4;
                    incomingData = new byte[incomingPacketSize];
                    sizeSet = true;
                }
                else
                    incomingBytes.AddRange(data.ToList());
            }

            if(sizeSet && incomingBytes.Count < incomingPacketSize)
            {
                incomingBytes.AddRange(data.ToList());

                if(incomingBytes.Count >= incomingPacketSize)
                {

                    byte[] dataBytes = incomingBytes.ToArray();

                    //Read the messagetype
                    int messageTypeInt = BitConverter.ToInt32(dataBytes, readIndex);
                    readIndex += 4;
                    MessageType messageType = (MessageType)messageTypeInt;


                    string messageJSON = "";

                    switch (messageType)
                    {
                        case MessageType.Create:
                            messageJSON = Encoding.Default.GetString(dataBytes, readIndex, dataBytes.Length-readIndex);
                            GameserverInstance gameserverToCreate = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                            ServerManager.CreateGameServer(gameserverToCreate, this);                            
                            break;
                        case MessageType.Register:
                            messageJSON = Encoding.Default.GetString(dataBytes, readIndex, dataBytes.Length - readIndex);
                            GameserverInstance gameserverToRegister = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                            ServerManager.Register(gameserverToRegister, this);
                            break;
                        case MessageType.Ready:
                            messageJSON = Encoding.Default.GetString(dataBytes, readIndex, dataBytes.Length - readIndex);
                            GameserverInstance gameserverReady = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                            ServerManager.RecieveGameserverReady(gameserverReady, this);
                            break;
                        default:
                            break;
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
