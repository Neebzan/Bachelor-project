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
        public static List<Client> ConnectedClients = new List<Client>();

        public static void ListenForClients (int port) {
            try {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Listening for clients on port {port}");
                while (true) {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Client client = new Client(tcpClient);

                    ConnectedClients.Add(client);
                    Console.WriteLine($"Client connected! from {tcpClient.Client.RemoteEndPoint.ToString()}");
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
