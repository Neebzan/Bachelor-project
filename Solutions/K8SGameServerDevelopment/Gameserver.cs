using Newtonsoft.Json;
using ServerManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace K8SGameServerDevelopment {
    public static class Gameserver {

        public static GameserverInstance Instance { get; set; }
        public static List<GameserverClient> ConnectedClients = new List<GameserverClient>();
        public static GameserverClient ConnectedServerManager = null;
        public static readonly int _minPort = 27005;
        public static readonly int _maxPort = 27100;
        //public static readonly int _serverManagerPort = 27002;
        public static readonly int _serverManagerPort = 30007;
        //public static readonly string _serverManagerIP = "servermanager-gameserver-service";
        public static readonly string _serverManagerIP = "212.10.51.254";
        private static Stopwatch stopwatch = new Stopwatch();

        public static void Init () {
            TcpListener listener = StartListenerWithAvailablePort(_minPort, _maxPort);
            if (listener != null) {
                Task.Run(() => ListenForClients(ref listener));
                SendRegisterRequest(listener);
                Console.WriteLine($"Waiting for configuration");
                Task.Run(() => Game());
            }
            else {
                Console.WriteLine("No available ports");
            }
        }

        private static void ListenForClients (ref TcpListener listener) {
            try {
                while (true) {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    GameserverClient client = new GameserverClient(tcpClient);

                    ConnectedClients.Add(client);
                    if (ConnectedClients.Count == 1) {
                        stopwatch.Start();
                    }
                    Console.WriteLine($"Client connected! from {tcpClient.Client.RemoteEndPoint.ToString()}");
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private static void Game () {
            while (true) {
                if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds > 10000) {
                    Environment.Exit(0);
                }
                Thread.Sleep(100);
            }
        }

        public static void SendRegisterRequest (TcpListener listener) {
            Console.WriteLine("Sending register request");
            TcpClient tcpClient = new TcpClient(_serverManagerIP, _serverManagerPort);
            ConnectedServerManager = new GameserverClient(tcpClient);

            GameserverInstance gameserverInstance = new GameserverInstance() {
                GameserverID = "", // Read from kubernetes pod
                Port = ((IPEndPoint)listener.LocalEndpoint).Port,
                IP = "212.10.51.254"
            };

            string JSON = JsonConvert.SerializeObject(gameserverInstance);

            List<byte> dataList = Encoding.Default.GetBytes(JSON).ToList();

            int method = (int)MessageType.Register;

            dataList.InsertRange(0, BitConverter.GetBytes(method));

            byte [ ] data = dataList.ToArray();

            tcpClient.GetStream().BeginWrite(data, 0, data.Length, null, null);
        }

        public static void RecieveConfigurationRequest (GameserverInstance configuration) {
            Console.WriteLine("Server configuration recieved");
            Console.WriteLine("Configuring server..");
            Instance = configuration;
            Console.WriteLine("Server configured");
            //StartServer(); // Some unity logic with the given configuration

            // After server is started and configured, send ready update to ServerManager
            SendReadyState();
        }



        private static void SendReadyState () {
            Console.WriteLine("Sending ready state to ServerManager");

            Instance.GameState = GameState.Running;
            string JSON = JsonConvert.SerializeObject(Instance);

            List<byte> dataList = Encoding.Default.GetBytes(JSON).ToList();

            int method = (int)MessageType.Ready;

            dataList.InsertRange(0, BitConverter.GetBytes(method));

            byte [ ] data = dataList.ToArray();

            ConnectedServerManager.TcpClient.GetStream().BeginWrite(data, 0, data.Length, null, null);
        }

        public static TcpListener StartListenerWithAvailablePort (int minPort, int maxPort) {
            try {
                if (minPort < maxPort) {
                    TcpListener listener = new TcpListener(IPAddress.Any, minPort);
                    listener.Start();
                    return listener;
                }
                else {
                    return null;
                }
            }
            catch (Exception e) {
                if (e is SocketException) {
                    return StartListenerWithAvailablePort(minPort + 1, maxPort);
                }
                else {
                    throw;
                }
            }
        }
    }
}
