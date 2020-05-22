using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerManager {
    public static class GameserverConnectionManager {

        public static List<Client> Gameservers = new List<Client>();

        public static void ListenForGameservers (int port) {
            try {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Listening for gameservers on port {port}");
                while (true) {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Client client = new Client(tcpClient);

                    Gameservers.Add(client);
                    Console.WriteLine($"Gameserver connected! from {tcpClient.Client.RemoteEndPoint.ToString()}");
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
