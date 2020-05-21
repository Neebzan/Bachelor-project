using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace GameServerDecoy {
    class Program {
        static void Main (string [ ] args) {
            Connect();
            //TestProcess();

            Console.ReadKey();
        }

        //public static void TestProcess () {
        //    Process process = new Process();
        //    process.StartInfo = new ProcessStartInfo("ipconfig", "/all");
        //    process.Start();
        //}

        public static void Connect () {
            TcpClient client = new TcpClient("212.10.51.254", 30832);
            //TcpClient client = new TcpClient("localhost", 27001);
            if (client.Connected) {
                Console.WriteLine("Connected");
            }
            else {
                Console.WriteLine("Not connected");
            }
        }
    }
}
