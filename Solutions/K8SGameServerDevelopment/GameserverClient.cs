using Newtonsoft.Json;
using ServerManager;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace K8SGameServerDevelopment {
    public class GameserverClient {
        private int _bufferSize = 4096;
        private byte [ ] _buffer;
        public TcpClient TcpClient;

        public GameserverClient (TcpClient client) {
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
                    case MessageType.Configure:
                        messageJSON = Encoding.Default.GetString(messageData);
                        GameserverInstance configuration = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
                        Gameserver.RecieveConfigurationRequest(configuration);
                        break;
                    case MessageType.Ready:
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
