using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager {
    public enum MessageType {
        Create,
        Register,
        Configure,
        Ready
    }

    public static class ServerManager {
        private readonly static int _clientPort = 27001;
        private readonly static int _gameserverPort = 27002;

        private static string _k8sRessource = "Files/GameServerPod.yaml";

        //private static List<GameserverInstance> GameserverInstances = new List<GameserverInstance>();
        private static Dictionary<GameserverInstance, Client> GameserverInstances = new Dictionary<GameserverInstance, Client>();

        /// <summary>
        /// Setup ServerManager to start accepting clients
        /// </summary>
        internal static void Init () {
            Console.WriteLine("Started");
            //Console.WriteLine("Running with kubectl: ");
            //PrintKubectlVersion();

            Task.Run(() => ClientConnectionManager.ListenForClients(_clientPort));
            Task.Run(() => GameserverConnectionManager.ListenForGameservers(_gameserverPort));
        }

        /// <summary>
        /// Prints the installed version of kubectl
        /// </summary>
        private static void PrintKubectlVersion () {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("kubectl", "version --short");
            process.Start();
        }

        public static void Register (GameserverInstance gameserverToRegister, Client client) {
            Console.WriteLine($"Started server on {gameserverToRegister.IP + ":" + gameserverToRegister.Port}");

            GameserverInstance instance = null;

            // Find a server that is now ready
            lock (GameserverInstances) {
                foreach (GameserverInstance server in GameserverInstances.Keys) {
                    if (server.GameState == GameState.Starting) {
                        instance = server;
                        break;
                    }
                }
            }

            // We found a starting server
            // Merge with information of started server
            if (instance != null) {
                instance.GameserverID = gameserverToRegister.GameserverID;
                instance.Port = gameserverToRegister.Port;
                instance.IP = gameserverToRegister.IP;
                instance.GameState = GameState.Configuring;
                SendServerConfiguration(instance, client);
            }
        }

        public static void RecieveGameserverReady (GameserverInstance gameserverReady, Client client) {
            Console.WriteLine($"Gameserver {gameserverReady.GameserverID} ready recieved");
            GameserverInstance instance = null;

            // Find a server that is now ready
            lock (GameserverInstances) {
                foreach (GameserverInstance server in GameserverInstances.Keys) {
                    if (server.GameserverID == gameserverReady.GameserverID) {
                        instance = server;
                        break;
                    }
                }
            }

            if (instance != null) {
                Console.WriteLine($"Sending gameserver {gameserverReady.GameserverID} information to player");
                SendServerToClient(instance);
            }
        }

        private static void SendServerToClient (GameserverInstance instance) {
            Console.WriteLine($"Sending server {instance.GameserverID} information to client");

            string JSON = JsonConvert.SerializeObject(instance);

            List<byte> dataList = Encoding.Default.GetBytes(JSON).ToList();

            int method = (int)MessageType.Ready;

            dataList.InsertRange(0, BitConverter.GetBytes(method));

            byte [ ] data = dataList.ToArray();

            lock (GameserverInstances) {
                GameserverInstances [ instance ].TcpClient.GetStream().BeginWrite(data, 0, data.Length, null, null);
            }
        }

        private static void SendServerConfiguration (GameserverInstance instanceToConfigure, Client client) {
            Console.WriteLine($"Sending server configuration to {instanceToConfigure.GameserverID}");

            string JSON = JsonConvert.SerializeObject(instanceToConfigure);

            List<byte> dataList = Encoding.Default.GetBytes(JSON).ToList();

            int method = (int)MessageType.Configure;

            dataList.InsertRange(0, BitConverter.GetBytes(method));

            byte [ ] data = dataList.ToArray();

            client.TcpClient.GetStream().BeginWrite(data, 0, data.Length, null, null);
        }

        /// <summary>
        /// Creates a game server instance
        /// </summary>
        public static void CreateGameServer (GameserverInstance gameServerInstance, Client client) {
            Console.WriteLine("Creating server instance");
            gameServerInstance.GameState = GameState.Starting;

            lock (GameserverInstances) {
                GameserverInstances.Add(gameServerInstance, client);
            }

            StartGameserverOnCluster();


        }

        private static void StartGameserverOnCluster () {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("kubectl", $"create -f {_k8sRessource}");
            process.Start();
        }
    }
}
