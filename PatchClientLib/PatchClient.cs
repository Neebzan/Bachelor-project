using ConnectionHandlerLib;
using GlobalConfigs;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PatchClientLib
{
    public class PatchClient
    {
        TcpClient client;

        string ip;
        int port;

        public string[] versions;

        bool running;

        public PatchClient(int port, string ip = "127.0.0.1")
        {
            this.ip = ip;
            this.port = port;
        }

        ~PatchClient()
        {
            
        }

        public void ConnectToServer()
        {
            client = new TcpClient(ip, port);
        }

        public void DisconectFromServer()
        {

        }

        public void HandleResponse()
        {
            while (ConnectionHandler.Connected(client))
            {

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

            Console.WriteLine("Awaiting response");
            bool done = false;
            while (ConnectionHandler.Connected(client) && !done)
            {
                if (client.GetStream().DataAvailable)
                {
                    Console.WriteLine("Incoming response");
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));

                    Console.WriteLine("Response recieved");

                    Console.WriteLine("Response was;");
                    Console.WriteLine(data.TestString);

                    done = true;
                }
            }
        }

        public void RequestFilesVersions()
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.AvailableVersions
            };
            ConnectionHandler.SendObject(model, client);
            Console.WriteLine("Version request send to server");

            Console.WriteLine("Awaiting response");
            bool done = false;
            while (ConnectionHandler.Connected(client) && !done)
            {
                if (client.GetStream().DataAvailable)
                {
                    Console.WriteLine("Incoming response");
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));
                    versions = data.Versions;

                    Console.WriteLine("Response recieved");
                    Console.WriteLine("Versions available:");
                    for (int i = 0; i < data.Versions.Length; i++)
                    {
                        Console.WriteLine(data.Versions[i]);
                    }
                    done = true;
                }
            }
        }

        public void RequestAvailableVersions()
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.AvailableVersions
            };
            ConnectionHandler.SendObject(model, client);
            Console.WriteLine("Version request send to server");

            Console.WriteLine("Awaiting response");
            bool done = false;
            while (ConnectionHandler.Connected(client) && !done)
            {
                if (client.GetStream().DataAvailable)
                {
                    Console.WriteLine("Incoming response");
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));
                    versions = data.Versions;

                    Console.WriteLine("Response recieved");
                    Console.WriteLine("Versions available:");
                    for (int i = 0; i < data.Versions.Length; i++)
                    {
                        Console.WriteLine(data.Versions[i]);
                    }
                    done = true;
                }
            }
        }
    }
}
