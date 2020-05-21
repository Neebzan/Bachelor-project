using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ServerManager {
    class Program {
        static List<TcpClient> tcpClients = new List<TcpClient>();

        static void Main (string [ ] args) {
            Console.WriteLine("Started");
            Console.WriteLine("Listening for connections");
            ListenForConnections();
        }

        public static void ListenForConnections () {
            TcpListener listener = new TcpListener(IPAddress.Any, 27001);
            listener.Start();
            while (true) {
                TcpClient client = listener.AcceptTcpClient();
                tcpClients.Add(client);
                Console.WriteLine($"Connected! from {client.Client.RemoteEndPoint.ToString()}");
                Console.WriteLine("Begin K8S API attempt");
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo("kubectl", "get pods");
                process.Start();
            }
        }
    }

}
