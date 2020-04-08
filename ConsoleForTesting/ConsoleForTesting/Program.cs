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

            Console.WriteLine("Connected");
            //Console.WriteLine("Connected");
            //PatchDataModel model = new PatchDataModel()
            //{
            //    RequestType = PatchNetworkRequest.AvailableVersions
            //};

            PatchClient pClient = new PatchClient(10000);

            pClient.ConnectToServer();

            pClient.RequestConnectionTest();

            Console.ReadKey();
        }
    }
}
