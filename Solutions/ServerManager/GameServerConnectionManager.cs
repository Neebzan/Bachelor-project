using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerManager {
    public static class GameserverConnectionManager {

        private static List<GameServerInstance> _gameservers = new List<GameServerInstance>();

        public static void ListenForGameservers (int port) {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Listening for connections");
            while (true) {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine($"Gameserver connected from {client.Client.RemoteEndPoint.ToString()}");
            }
        }
    }
}
