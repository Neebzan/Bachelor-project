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
        string masterDirectory = "";
        string[] versions;
        bool running;

        public PatchServer()
        {
        }

        public void UpdateMasterFiles()
        {
            versions = ChecksumTool.GetAvailableFolders(masterDirectory);
            masterFiles = new Dictionary<string, Dictionary<string, string>>();

            for (int i = 0; i < versions.Length; i++)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                ChecksumTool.GetFilesDictionary(out temp, versions[i]);
                masterFiles.Add(versions[i], temp);
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
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));

                    switch (data.RequestType)
                    {
                        case PatchNetworkRequest.AvailableVersions:
                            SendAvailableVersions(client);
                            break;
                    }
                }
            }

            lock (clients)
                clients.Remove(client);
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
