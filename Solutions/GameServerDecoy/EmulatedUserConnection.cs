using Newtonsoft.Json;
using ServerManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace GameServerDecoy {
    static class EmulatedUserConnection {

        static EmulatedPlayerClient ServerManagerClient = null;
        static EmulatedPlayerClient GameClient = null;

        public static void Init () {
            SendCreateServerRequest();
        }

        public static void SendCreateServerRequest () {
            Console.WriteLine($"Sending request to create server");

            ServerManagerClient = new EmulatedPlayerClient(new TcpClient("212.10.51.254", 30006));
            //ServerManagerClient = new Client(new TcpClient("212.10.51.254", 27001));

            GameserverInstance gameserverInstance = new GameserverInstance();

            // Get the name generated for this pod
            gameserverInstance.ServerName = "Test name";

            // Serialize GameserverInstance
            string JSON = JsonConvert.SerializeObject(gameserverInstance);

            Packet packet = new Packet((int)MessageType.Create);
            packet.Write(JSON);
            packet.WriteLength();

            ServerManagerClient.TcpClient.GetStream().BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            Console.WriteLine("Waiting for server response");
        }

        public static void ReceiveServerInfo (GameserverInstance gameserverInstance) {
            Console.WriteLine($"Received server info");
            Console.WriteLine($"ServerID {gameserverInstance.GameserverID}");
            Console.WriteLine($"Game name {gameserverInstance.ServerName}");
            Console.WriteLine($"Server IP {gameserverInstance.IP}");
            Console.WriteLine($"Server Port {gameserverInstance.Port}");

            ConnectToServer(gameserverInstance);
        }

        public static void ConnectToServer (GameserverInstance gameserverInstance) {
            Console.WriteLine("Connecting to game server");
            GameClient = new EmulatedPlayerClient(new TcpClient(gameserverInstance.IP, gameserverInstance.Port));
            Console.WriteLine("Connected!");
        }
    }
}

