using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ServerManager {
    public class ClientMessageRecievedEventHandler {
        public TcpClient Client { get; set; }
        public ClientMessageType MessageType { get; set; }

        public object Message;
    }

    public static class ClientConnectionManager {
        public static event EventHandler<ClientMessageRecievedEventHandler> ClientMessageRecieved;
        public static void ListenForClients (int port) {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (true) {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine($"Client connected! from {client.Client.RemoteEndPoint.ToString()}");

                // Handle message type
                HandleClientMessages(client);
            }
        }

        private static void HandleClientMessages (TcpClient client) {
            // Decode message format
            // Placeholder
            ClientMessageType clientMessage = ClientMessageType.CreateServer;

            ClientMessageRecieved?.Invoke(null, new ClientMessageRecievedEventHandler() { Client = client, MessageType = clientMessage });
        }
    }
}
