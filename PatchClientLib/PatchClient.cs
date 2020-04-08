using ChecksumHandlerLib;
using ConnectionHandlerLib;
using GlobalConfigs;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PatchClientLib
{
    public class PatchClient
    {
        TcpClient client;

        string ip;
        int port;

        public string[] serverVersions = new string[0];
        public Dictionary<string, Dictionary<string, string>> InstalledVersions = new Dictionary<string, Dictionary<string, string>>();
        string installPath;

        bool running;

        public PatchClient(int port, string ip = "127.0.0.1")
        {
            this.ip = ip;
            this.port = port;
        }

        ~PatchClient()
        {
            running = false;
        }

        public void ConnectToServer(string installPath = "ClientFiles")
        {
            this.installPath = installPath;
            client = new TcpClient(ip, port);
            Task.Run(() => HandleResponse());
        }

        public void DisconectFromServer()
        {
            running = false;
        }

        public void UpdateCurrentInstallations()
        {
            InstalledVersions = ChecksumTool.GetInstallationsAtPath(installPath);
        }

        public void HandleResponse()
        {
            running = true;
            while (ConnectionHandler.Connected(client) && running)
            {
                if (client.GetStream().DataAvailable)
                {
                    Console.WriteLine("Incoming response");
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));

                    switch (data.RequestType)
                    {
                        case PatchNetworkRequest.AvailableVersions:
                            HandleAvailableVersionsResponse(data);
                            break;
                        case PatchNetworkRequest.CheckFiles:
                            break;
                        case PatchNetworkRequest.TestConnection:
                            HandleConnectionTestResponse(data);
                            break;
                    }
                }
            }
        }
        public void RequestConnectionTest()
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.TestConnection
            };
            ConnectionHandler.SendObject(model, client);
            Console.WriteLine("Connection test send to server"); 
        }

        private void HandleConnectionTestResponse(PatchDataModel data)
        {
            Console.WriteLine("Response recieved");
            Console.WriteLine("Response was;");
            Console.WriteLine(data.TestString);
        }

        public void RequestAvailableVersions()
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.AvailableVersions
            };
            ConnectionHandler.SendObject(model, client);
            Console.WriteLine("Version request send to server");
        }

        private void HandleAvailableVersionsResponse(PatchDataModel data)
        {
            serverVersions = data.Versions;
            Console.WriteLine("Response recieved");
            Console.WriteLine("Versions available:");
            for (int i = 0; i < data.Versions.Length; i++)
            {
                Console.WriteLine(data.Versions[i]);
            }
        }
    }
}
