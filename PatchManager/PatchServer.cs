using ChecksumHandlerLib;
using ConnectionHandlerLib;
using GlobalConfigs;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PatchManager
{
    public class PatchServer
    {
        List<TcpClient> clients = new List<TcpClient>();
        Dictionary<string, Dictionary<string, string>> masterFiles;
        string masterDirectory = "TestFiles";
        string[] versions;
        bool running;

        public PatchServer()
        {
        }

        ~PatchServer()
        {
            running = false;
        }

        public void UpdateMasterFiles()
        {
            masterFiles = new Dictionary<string, Dictionary<string, string>>();
            versions = new string[] { "0" };

            versions = ChecksumTool.GetAvailableFolders(masterDirectory);
            for (int i = 0; i < versions.Length; i++)
            {
                Console.WriteLine("Find files for: " + versions[i]);
                Console.WriteLine("At path: " + masterDirectory + @"\" + versions[i]);
                Dictionary<string, string> temp = new Dictionary<string, string>();
                ChecksumTool.GetFilesDictionary(out temp, masterDirectory + @"\" + versions[i]);
                masterFiles.Add(versions[i], temp);
            }

            Console.WriteLine("Versions was:");
            for (int i = 0; i < versions.Length; i++)
            {
                Console.WriteLine(versions[i]);
            }
        }

        public void Start()
        {
            UpdateMasterFiles();
            if (!running)
                Task.Run(() => HandleIncomingConnections());
        }

        public void Stop()
        {
            running = false;
        }

        public void HandleIncomingConnections()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Configs.PATCH_SERVER_PORT);
            listener.Start();
            running = true;
            Console.WriteLine("Started handling incoming connections");
            while (running)
            {
                TcpClient client = listener.AcceptTcpClient();

                lock (clients)
                {
                    Console.WriteLine("New connection added!");
                    clients.Add(client);
                    Task.Run(() => HandleClientConnection(client));
                }
            }
            Console.WriteLine("Stopped handling incoming connections");
        }

        public void HandleClientConnection(TcpClient client)
        {
            //While the client is still connected, await communication
            while (ConnectionHandler.Connected(client))
            {
                //Wait for data from client
                if (client.GetStream().DataAvailable)
                {
                    Console.WriteLine("Recieving request");
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));

                    switch (data.RequestType)
                    {
                        case PatchNetworkRequest.AvailableVersions:
                            SendAvailableVersions(client);
                            Console.WriteLine("Versions send to client");
                            break;
                        case PatchNetworkRequest.CheckFiles:
                            break;
                        case PatchNetworkRequest.TestConnection:
                            SendTestConnectionResponse(client);
                            Console.WriteLine("Send test response to client");
                            break;
                    }
                }
            }

            lock (clients)
                clients.Remove(client);
        }

        private void SendTestConnectionResponse(TcpClient client)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.TestConnection,
                Connected = true,
                TestString = "You're in!"
            };
            byte[] data = ConnectionHandler.ConvertToBytes<PatchDataModel>(model);
            client.GetStream().Write(data, 0, data.Length);
        }

        private void SendAvailableVersions(TcpClient client)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.AvailableVersions,
                Versions = versions
            };
            byte[] data = ConnectionHandler.ConvertToBytes<PatchDataModel>(model);
            client.GetStream().Write(data, 0, data.Length);
        }
    }
}
