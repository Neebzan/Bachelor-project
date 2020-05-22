using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerManager {
    public class Client {
        private int _bufferSize = 4096;
        private byte [ ] _buffer;
        public TcpClient TcpClient;

        public Client (TcpClient client) {
            _buffer = new byte [ _bufferSize ];
            TcpClient = client;

            TcpClient.ReceiveBufferSize = _bufferSize;
            TcpClient.GetStream().BeginRead(_buffer, 0, _bufferSize, TcpCallback, null);
        }

        public void TcpCallback (IAsyncResult asyncResult) {
            try {
                int messageLength = TcpClient.GetStream().EndRead(asyncResult);
                byte [ ] messageData = new byte [ messageLength ];

                Array.Copy(_buffer, 4, messageData, 0, messageLength - 4);

                int messageTypeInt = BitConverter.ToInt32(_buffer, 0);
                MessageType messageType = (MessageType)messageTypeInt;

                TcpClient.GetStream().BeginRead(_buffer, 0, _bufferSize, TcpCallback, null);


                string messageJSON = "";
                
                switch (messageType) {
                    case MessageType.Create:
                        messageJSON = Encoding.Default.GetString(messageData);
                        GameserverInstance gameserverToCreate = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                        ServerManager.CreateGameServer(gameserverToCreate, this);
                        break;
                    case MessageType.Register:
                        messageJSON = Encoding.Default.GetString(messageData);
                        GameserverInstance gameserverToRegister = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                        ServerManager.Register(gameserverToRegister, this);
                        break;
                    case MessageType.Ready:
                        messageJSON = Encoding.Default.GetString(messageData);
                        GameserverInstance gameserverReady = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                        ServerManager.RecieveGameserverReady(gameserverReady, this);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
