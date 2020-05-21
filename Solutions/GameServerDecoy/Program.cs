using System;
using System.Net.Sockets;

namespace GameServerDecoy {
    class Program {
        static void Main (string [ ] args) {
            TcpClient client = new TcpClient("212.10.51.254", 27001);
            if (client.Connected) {
                Console.WriteLine("Connected");
            }
            else {
                Console.WriteLine("Not connected");
            }
            Console.ReadKey();
        }
    }
}
