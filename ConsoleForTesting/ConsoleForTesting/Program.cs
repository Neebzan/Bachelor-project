using ChecksumHandlerLib;
using ConnectionHandlerLib;
using Models;
using Newtonsoft.Json;
using PatchClientLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            //ChecksumTool.GetFilesDictionary();
            //Dictionary<string, string> testDic = new Dictionary<string, string>();
            //ChecksumTool.GetFilesDictionary(out testDic, @"C:\Users\Henrik\Desktop\backups");
            //ChecksumTool.GetFilesDictionary(out testDic, @"TestFolder\TestFolder2");
            //ChecksumTool.GetFilesDictionary(out testDic);
            //string[] t = ChecksumTool.GetAvailableFolders();
            //for (int i = 0; i < t.Length; i++)
            //{
            //    ChecksumTool.GetFilesDictionary(out testDic, t[i]);
            //}
            //FileDataModel testModel = new FileDataModel();
            //string json = JsonConvert.SerializeObject(testModel);

            //Console.WriteLine("Yeet");
            //ChecksumTool.HelloWorld();
            //TcpClient client = new TcpClient("127.0.0.1", 10000);
            //TcpClient client = new TcpClient("212.10.51.254", 31000);
            //TcpClient client = new TcpClient("10.108.67.120", 80);

            //Console.WriteLine("Connected");
            //PatchDataModel model = new PatchDataModel()
            //{
            //    RequestType = PatchNetworkRequest.AvailableVersions
            //};

            //PatchClient pClient = new PatchClient(31000, "212.10.51.254");
            PatchClientTest();

            //Console.ReadKey();
        }

        static void PatchClientTest()
        {
            bool running = true;
            PatchClient pClient = new PatchClient(10000);
            pClient.ConnectToServer();
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            WriteCommands(pClient);
            while (running)
            {
                switch (key.Key)
                {
                    case ConsoleKey.D0:
                        WriteCommands(pClient);
                        break;
                    case ConsoleKey.D1:
                        WriteCommands(pClient);
                        pClient.UpdateCurrentInstallations();
                        break;
                    case ConsoleKey.D2:
                        WriteCommands(pClient);
                        pClient.RequestConnectionTest();
                        break;
                    case ConsoleKey.D3:
                        WriteCommands(pClient);
                        pClient.RequestAvailableVersions();
                        break;
                    case ConsoleKey.Escape:
                        running = false;
                        break;
                }
                key = Console.ReadKey();
                Console.Clear();
            }
        }

        static void WriteCommands(PatchClient pClient)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;

            Console.WriteLine("Actions available:");
            Console.WriteLine("0. Update shown info");
            Console.WriteLine("1. Check for installed versions");
            Console.WriteLine("2. Test connection to server");
            Console.WriteLine("3. Request available versions from server");

            Console.WriteLine("\n----------------------------------\n");
            PatchClientInfo(pClient);


            Console.WriteLine("\n----------------------------------\n");
            Console.WriteLine("Log:\n");
        }

        static void PatchClientInfo(PatchClient client)
        {
            Console.WriteLine("Information stored in patchclient:");
            Console.WriteLine("Versions available on server: " + client.serverVersions.Length);
            for (int i = 0; i < client.serverVersions.Length; i++)
            {
                Console.WriteLine(client.serverVersions[i]);
            }
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Locally found versions:" + client.InstalledVersions.Count);
            foreach (var item in client.InstalledVersions)
            {
                Console.WriteLine(item.Key);
            }
        }
    }
}
