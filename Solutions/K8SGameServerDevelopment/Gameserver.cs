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
        public static bool Running { get; internal set; } = true;

        public static List<GameserverClient> ConnectedClients = new List<GameserverClient>();
        public static GameserverClient ConnectedServerManager = null;
        public static readonly int _minPort = 27005;
        public static readonly int _maxPort = 27100;
        //public static readonly int _serverManagerPort = 27002;
        public static readonly int _serverManagerPort = 30007;
        //public static readonly string _serverManagerIP = "servermanager-gameserver-service.default.svc.cluster.local";
        public static readonly string _serverManagerIP = "212.10.51.254";
        private static Stopwatch stopwatch = new Stopwatch();
        private static string GameID = "";


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
            Console.WriteLine("\nGame started");
            while (true) {
                if (stopwatch.IsRunning && stopwatch.ElapsedMilliseconds > 10000) {
                    Running = false;
                }
                Thread.Sleep(100);
            }
        }

        public static void SendRegisterRequest (TcpListener listener) {
            TcpClient tcpClient = new TcpClient(_serverManagerIP, _serverManagerPort);
            ConnectedServerManager = new GameserverClient(tcpClient);

            Console.WriteLine("Getting game ID..");

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo() {
                FileName = "printenv",
                Arguments = "SESSION_NAME",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            string gameIdentifier = "";

            // Read result from process
            p.Start();
            while (!p.StandardOutput.EndOfStream) {
                gameIdentifier = p.StandardOutput.ReadLine();
            }

            p.WaitForExit();

            Console.WriteLine("GameID: " + gameIdentifier);
            Console.WriteLine("\n");

            Console.WriteLine("Creating game instance..");
            Instance = new GameserverInstance() {
                GameserverID = "", // Read from kubernetes pod
                Port = ((IPEndPoint)listener.LocalEndpoint).Port,
                IP = "212.10.51.254"
            };


            if (gameIdentifier.Contains("game-"))
                Instance.GameserverID = gameIdentifier;
            else
                Instance.GameserverID = "Default";

            Console.WriteLine("Instance created");
            Console.WriteLine($"ServerID {Instance.GameserverID}");
            Console.WriteLine($"Server IP {Instance.IP}");
            Console.WriteLine($"Server Port {Instance.Port}");
            Console.WriteLine("\n");

            string JSON = JsonConvert.SerializeObject(Instance);

            Packet packet = new Packet((int)MessageType.Register);
            packet.Write(JSON);
            packet.WriteLength();

            Console.WriteLine($"Sending game instance to server manager, to be configured..");
            tcpClient.GetStream().BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
        }

        public static void RecieveConfigurationRequest (GameserverInstance configuration) {
            Console.WriteLine("Server configuration recieved");
            Console.WriteLine("Configuring server..");
            Instance = configuration;
            Console.WriteLine("Server configured");
            Console.WriteLine("\n");
            SendReadyState();
        }

        private static void SendReadyState () {
            Console.WriteLine("Sending ready state to ServerManager");

            Instance.GameState = GameState.Running;
            string JSON = JsonConvert.SerializeObject(Instance);

            Packet packet = new Packet((int)MessageType.Ready);
            packet.Write(JSON);
            packet.WriteLength();

            ConnectedServerManager.TcpClient.GetStream().BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
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
