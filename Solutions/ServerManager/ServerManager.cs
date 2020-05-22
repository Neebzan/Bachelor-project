using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager
{
    public enum MessageType
    {
        Create,
        Register,
        Configure,
        Ready
    }

    public static class ServerManager
    {
        private readonly static int _clientPort = 27001;
        private readonly static int _gameserverPort = 27002;

        private static string _k8sRessource = "Files/GameServerPod.yaml";

        private static List<GameserverInstance> LiveGameInstances = new List<GameserverInstance>();
        private static Dictionary<GameserverInstance, Client> ConfiguringGameserverInstances = new Dictionary<GameserverInstance, Client>();

        /// <summary>
        /// Setup ServerManager to start accepting clients
        /// </summary>
        internal static void Init()
        {
            Console.WriteLine("Started");
            //Console.WriteLine("Running with kubectl: ");
            //PrintKubectlVersion();

            Task.Run(() => ClientConnectionManager.ListenForClients(_clientPort));
            Task.Run(() => GameserverConnectionManager.ListenForGameservers(_gameserverPort));
        }

        /// <summary>
        /// Prints the installed version of kubectl
        /// </summary>
        private static void PrintKubectlVersion()
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("kubectl", "version --short");
            process.Start();
        }

        public static void ReceiveRegisterRequest(GameserverInstance gameserverToRegister, Client client)
        {
            Console.WriteLine($"Started server on {gameserverToRegister.IP + ":" + gameserverToRegister.Port}");

            GameserverInstance instance = null;

            // Find a server that is now ready
            lock (ConfiguringGameserverInstances)
            {
                foreach (GameserverInstance server in ConfiguringGameserverInstances.Keys)
                {
                    if (server.GameState == GameState.Starting)
                    {
                        instance = server;
                        break;
                    }
                }
            }

            // We found a starting server
            // Merge with information of started server
            if (instance != null)
            {
                instance.GameserverID = gameserverToRegister.GameserverID;
                instance.Port = gameserverToRegister.Port;
                instance.IP = gameserverToRegister.IP;
                instance.GameState = GameState.Configuring;
                SendServerConfiguration(instance, client);
            }
        }

        public static void RecieveGameserverReady(GameserverInstance gameserverReady, Client client)
        {
            Console.WriteLine($"Gameserver {gameserverReady.GameserverID} ready recieved");
            GameserverInstance instance = null;

            // Find a server that is now ready
            lock (ConfiguringGameserverInstances)
            {
                foreach (GameserverInstance server in ConfiguringGameserverInstances.Keys)
                {
                    if (server.GameserverID == gameserverReady.GameserverID)
                    {
                        instance = gameserverReady;
                        break;
                    }
                }
            }

            if (instance != null)
            {
                SendServerToClient(instance);
            }
        }

        private static void SendServerToClient(GameserverInstance instance)
        {

            string JSON = JsonConvert.SerializeObject(instance);


            Packet packet = new Packet((int)MessageType.Ready);
            packet.Write(JSON);
            packet.WriteLength();

            lock (ConfiguringGameserverInstances)
            {
                Console.WriteLine($"Sending gameserver {instance.GameserverID} information to player");
                ConfiguringGameserverInstances [instance].TcpClient.GetStream().BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                LiveGameInstances.Add(instance);
                ConfiguringGameserverInstances.Remove(instance);

            }
        }

        public static void DisconnectClient(Client client)
        {
            lock (ConfiguringGameserverInstances)
                if (ConfiguringGameserverInstances.ContainsValue(client))
                {
                    var instance = ConfiguringGameserverInstances.First(x => x.Value == client).Key;
                    ConfiguringGameserverInstances.Remove(instance);
                    client.TcpClient.GetStream().Close();
                    client.TcpClient.Dispose();
                    client = null;
                }

        }

        private static void SendServerConfiguration(GameserverInstance instanceToConfigure, Client client)
        {
            Console.WriteLine($"Sending server configuration to {instanceToConfigure.GameserverID}");

            string JSON = JsonConvert.SerializeObject(instanceToConfigure);


            Packet packet = new Packet((int)MessageType.Configure);
            packet.Write(JSON);
            packet.WriteLength();

            client.TcpClient.GetStream().BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
        }

        /// <summary>
        /// Creates a game server instance
        /// </summary>
        public static void CreateGameServer(GameserverInstance gameServerInstance, Client client)
        {
            Console.WriteLine("Creating server instance");
            gameServerInstance.GameState = GameState.Starting;

            lock (ConfiguringGameserverInstances)
            {
                ConfiguringGameserverInstances.Add(gameServerInstance, client);
            }

            StartGameserverOnCluster();


        }

        private static void StartGameserverOnCluster()
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("kubectl", $"create -f {_k8sRessource}");
            process.Start();
        }
    }
}
