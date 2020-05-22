using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager {
    public static class ServerManager {
        private readonly static int _clientPort = 27001;
        private readonly static int _gameserverPort = 27002;

        private static string _k8sRessource = "Files/GameServerPod.yaml";

        /// <summary>
        /// Setup ServerManager to start accepting clients
        /// </summary>
        internal static void Init () {
            Console.WriteLine("Started");
            Console.WriteLine("Running with kubectl: ");
            PrintKubectlVersion();

            Task.Run(() => ClientConnectionManager.ListenForClients(_clientPort));
            ClientConnectionManager.ClientMessageRecieved += ClientConnectionManager_ClientMessageRecieved;

            Task.Run(() => GameserverConnectionManager.ListenForGameservers(_gameserverPort));
        }

        private static void ClientConnectionManager_ClientMessageRecieved (object sender, ClientMessageRecievedEventHandler e) {
            switch (e.MessageType) {
                case ClientMessageType.CreateServer:
                    // Feed relevant information, such as name and size
                    //CreateGameServer();
                    break;
                case ClientMessageType.JoinServer:
                    break;
                case ClientMessageType.DeleteServer:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Prints the installed version of kubectl
        /// </summary>
        private static void PrintKubectlVersion () {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("kubectl", "version --short");
            process.Start();
        }

        /// <summary>
        /// Creates a game server instance
        /// </summary>
        public static void CreateGameServer (GameServerInstance gameServerInstance) {
            Console.WriteLine("Creating server instance");
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("kubectl", $"create -f {_k8sRessource}");
            process.Start();
        }
    }
}
