using ChecksumHandlerLib;
using ConnectionHandlerLib;
using DatabaseLib;
using DatabaseREST.Models;
using GlobalConfigs;
using HelperTools;
using Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PatchClientLib;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            PatchClient.InstallPath = @"H:\Skole\Bachelor-project\ConsoleForTesting\ConsoleForTesting\bin\Debug\ClientFiles\welp";
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Matches testMatch = new Matches()
            //{   
            //    MapName = "map1",
            //    Begun = DateTime.Now,
            //    Ended = DateTime.Now.AddDays(1),
            //    Difficulty = 100

            //};

            //PlayedMatch testPlayedMatch = new PlayedMatch()
            //{
            //    Deaths = 5,
            //    Score = 999,
            //    Kills = 10,
            //    MatchId = 1,
            //    PlayerId = "neebz"
            //};

            //DBConnection.Instance().Insert(testMatch);
            //DBConnection.Instance().Insert(testPlayedMatch);

            //var h = PatchClient.CompleteCheck(new string[] {PatchClient.InstallPath });
            //var h = PatchClient.CompleteCheck(new string[0]);
            //h[0].InstallPath = PatchClient.InstallPath;

            DBConnection.Instance().InsertRandomData(1000000);

            //DBConnection.Instance().EFCORETest();


            //var gh = new InstallationDataModel()
            //{
            //    Status = InstallationStatus.NotInstalled,
            //    VersionBranch = VersionBranch.Beta,
            //    VersionName = ""
            //};

            //string[] paths = new string[] { @"H:\Skole\Bachelor-project\ConsoleForTesting\ConsoleForTesting\bin\Debug\ClientFiles\welp" };
            ////PatchClient._client = new TcpClient("localhost", 13000);
            //PatchClient._ip = "localhost";
            //PatchClient._port = 13000;


            //var tempInstallation = PatchClient.CompleteCheck(paths);

            //tempInstallation[1].InstallPath = @"H:\Skole\Bachelor-project\ConsoleForTesting\ConsoleForTesting\bin\Debug\ClientFiles\test";

            //PatchClient.DownloadMissingFilesNew(tempInstallation[1]);

            //PatchClient.RequestVersionMissingFiles(ref gh);
            //PatchClient._client.GetStream().Close();
            //PatchClient._client.Close();


            //Accounts acc = new Accounts()
            //{
            //    AccountId = "TestAcc222",
            //    Email = "some@email.com",
            //    PasswordHash = "somePW"
            //};
            //DBConnection.Instance().Insert(acc);

            //var q = new InstallationDataModel()
            //{
            //    VersionName = "0.1b",
            //    VersionBranch = VersionBranch.Beta,
            //    InstallPath = PatchClient.InstallPath,
            //    Status = InstallationStatus.NotInstalled
            //};


            //PatchClient.DownloadMissingFiles(h[0]);
            //PatchClient.DownloadMissingFiles(h[0]);

            //var t = ChecksumTool.GetInstalledVersion(PatchClient.InstallPath);
            //watch.Stop();
            //Console.WriteLine(ConsoleExtension.AddTimestamp("Elapsed: " + watch.ElapsedMilliseconds));
            //Thread.Sleep(4000);

            //var h = PatchClient.CompleteCheck(new string[] {
            //PatchClient.InstallPath });


            //var t = PatchClient.InstalledVersions;

            //Test();

            //var testLoadFromFile = h[0].GetModelFromFile();
            //h[0].SaveToFile();



            Console.ReadKey();
        }

        public static bool Login()
        {

            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("accounts/login", Method.POST);
            client.UseJson();
            //request.AddHeader("content-type", "application/json");
            //client.UseNewtonsoftJson();

            //hash password
            //string password = Utility.HashedString(_password);

            //Create object
            Accounts account = new Accounts()
            {
                AccountId = "neebz2",
                PasswordHash = "re"
            };
            //request.AddParameter("application/json", account, ParameterType.RequestBody);

            request.AddJsonBody(account);

            var response = client.Execute<TokenModel>(request);
            

            var h = JsonConvert.DeserializeObject<TokenModel>(response.Content);
            string fff = response.Content;
            return true;
        }

        public static async void Test()
        {
            var t = PatchClient.InstalledVersions;
            var p = t[1];
            p.InstallPath = @"H:\Skole\Bachelor-project\ConsoleForTesting\ConsoleForTesting\bin\Debug\ClientFiles\VersionTest1";
            PatchClient.DownloadMissingFiles(p);
            t = PatchClient.InstalledVersions;
        }

        //        static void DBTests()
        //        {
        //            AccountModel testModel = new AccountModel()
        //            {
        //                account_id = "Bleh3",
        //                first_name = "Mr Bleh2",
        //                last_name = "Sausage2",
        //                email = "yes@no.com2",
        //                password_hash = "Very secure PW2"
        //            };

        //            PlayerModel testPlayer = new PlayerModel()
        //            {
        //                player_id = "Bleh3"
        //            };

        //            MatchModel testMatch = new MatchModel()
        //            {
        //                map_name = "map1",
        //                begun = DateTime.Now,
        //                ended = DateTime.Now.AddDays(1),
        //                difficulty = 100

        //            };

        //            PlayedMatch testPlayedMatch = new PlayedMatch()
        //            {
        //                deaths = 5,
        //                score = 999,
        //                kills = 10,
        //                match_id = 2,
        //                player_id = "Bleh3"
        //            };

        //            ItemColor testColor = new ItemColor()
        //            {
        //                color_name = "blue",
        //                red = 0,
        //                green = 0,
        //                blue = 255
        //            };
        //            ItemType typeTest = new ItemType()
        //            {
        //                type_value = "test"
        //            };

        //            ItemModel testItemModel = new ItemModel()
        //            {
        //                aquire_date = DateTime.Now,
        //                item_color = "blue",
        //                item_name = "testItem",
        //                item_type = "test",
        //                owner_id = "Bleh3",
        //                quality = 0.5f
        //            };

        //            ItemModel t = DBConnection.Instance().Get<ItemModel>(1);

        //            Wears wearTest = new Wears()
        //            {
        //                player_id = "Bleh3",
        //                item_type = t.item_type
        //            };

        //            DBConnection.Instance().Insert(wearTest);
        //            //DBConnection.Instance().Insert(testColor);
        //            //DBConnection.Instance().Insert(typeTest);
        //            //DBConnection.Instance().Insert(testItemModel);
        //            //DBConnection.Instance().Insert(testModel);
        //            //DBConnection.Instance().Insert(testPlayer);
        //            //DBConnection.Instance().Insert(testMatch);
        //            //DBConnection.Instance().Insert(testPlayedMatch);
        //        }

        //        static void PatchClientTest()
        //        {
        //            bool running = true;
        //            PatchClient pClient = new PatchClient(30831, "212.10.51.254");
        //            pClient.ConnectToServer();
        //            ConsoleKeyInfo key = new ConsoleKeyInfo();
        //            WriteCommands(pClient);
        //            while (running)
        //            {
        //                switch (key.Key)
        //                {
        //                    case ConsoleKey.D0:
        //                        WriteCommands(pClient);
        //                        break;
        //                    case ConsoleKey.D1:
        //                        WriteCommands(pClient);
        //                        pClient.UpdateCurrentInstallations();
        //                        break;
        //                    case ConsoleKey.D2:
        //                        WriteCommands(pClient);
        //                        pClient.RequestConnectionTest();
        //                        break;
        //                    case ConsoleKey.D3:
        //                        WriteCommands(pClient);
        //                        pClient.RequestAvailableVersions();
        //                        break;
        //                    case ConsoleKey.D4:
        //                        WriteCommands(pClient);
        //                        pClient.RequestVerifyVersions();
        //                        break;
        //                    case ConsoleKey.D5:
        //                        WriteCommands(pClient);
        //                        pClient.RequestVersionMissingFiles("VersionTest1");
        //                        break;
        //                    case ConsoleKey.D6:
        //                        WriteCommands(pClient);
        //                        pClient.DownloadMissingFiles(pClient.InstalledVersions.FirstOrDefault(x => x.VersionName == "VersionTest1"));
        //                        break;
        //                    case ConsoleKey.D7:
        //                        WriteCommands(pClient);
        //                        FullPatch(pClient);
        //                        break;
        //                    case ConsoleKey.Escape:
        //                        running = false;
        //                        break;
        //                }
        //                key = Console.ReadKey();
        //                Console.Clear();
        //            }
        //        }

        //        static void WriteCommands(PatchClient pClient)
        //        {
        //            Console.CursorLeft = 0;
        //            Console.CursorTop = 0;

        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Actions available:");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("0. Update shown info");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("1. Check for installed versions");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("2. Test connection to server");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("3. Request available versions from server");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("4. Verify versions with server");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("5. Get missing files from server (VersionTest1)");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("6. Download missing files from server (VersionTest1)");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("7. Run full patch");


        //            Console.WriteLine(ConsoleExtension.AddTimestamp("\n----------------------------------\n");
        //            PatchClientInfo(pClient);


        //            Console.WriteLine(ConsoleExtension.AddTimestamp("\n----------------------------------\n");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Log:\n");
        //        }

        //        static void FullPatch(PatchClient client)
        //        {
        //            bool next = false;
        //            Action answerRecieved = new Action(() =>
        //            {
        //                next = true;
        //            });

        //            client.UpdateCurrentInstallations();
        //            Console.Clear();
        //            ConsoleKeyInfo key = new ConsoleKeyInfo();

        //            client.VersionsFromServerReceived += answerRecieved;
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Requesting versions from server!");
        //            client.RequestAvailableVersions();
        //            while (!next)
        //            {

        //            }
        //            client.VersionsFromServerReceived -= answerRecieved;
        //            next = false;
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Choose which version to patch");
        //            for (int i = 0; i < client.serverVersions.Length; i++)
        //            {
        //                Console.WriteLine(ConsoleExtension.AddTimestamp(i + 1 + ". " + client.serverVersions[i]);
        //            }
        //            int choice = -1;
        //            while (!next)
        //            {
        //                key = Console.ReadKey();
        //                switch (key.Key)
        //                {
        //                    case ConsoleKey.D1:
        //                        choice = 0;
        //                        next = true;
        //                        break;
        //                    case ConsoleKey.D2:
        //                        choice = 1;
        //                        next = true;
        //                        break;
        //                }
        //            }

        //            next = false;

        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Attempting to verify current intallation of: " + client.serverVersions[choice]);
        //            if (client.CheckIfVersionExists(client.serverVersions[choice]))
        //            {
        //                var localVersion = client.GetVersionModel(client.serverVersions[choice]);
        //                Console.WriteLine(ConsoleExtension.AddTimestamp("A version of {0} exists locally", client.serverVersions[choice]);
        //                Console.WriteLine(ConsoleExtension.AddTimestamp("Verifying version with server");

        //                client.VersionVerificationDone += answerRecieved;
        //                client.RequestVerifyVersion(client.serverVersions[choice]);
        //                Console.WriteLine(ConsoleExtension.AddTimestamp("Waiting for answer from server");
        //                while (!next)
        //                {

        //                }
        //                client.VersionVerificationDone -= answerRecieved;

        //                if (localVersion.Verified)
        //                {
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp(localVersion.VersionName + " was VERIFIED by the server, patching DONE!");
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Press any key to continue...");
        //                    Console.ReadKey();
        //                    return;
        //                }
        //                else
        //                {
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp(localVersion.VersionName + " was NOT VERIFIED by the server, patching DONE!");
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Requesting missing/broken files");
        //                    client.MissingFileListReceived += answerRecieved;
        //                    next = false;

        //                    client.RequestVersionMissingFiles(localVersion.VersionName);
        //                    while (!next)
        //                    {

        //                    }
        //                    client.MissingFileListReceived -= answerRecieved;
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Missing files recieved!");
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Attempting to download missing files");

        //                    client.DownloadDone += answerRecieved;
        //                    next = false;
        //                    client.DownloadMissingFiles(localVersion);
        //                    while (!next)
        //                    {

        //                    }
        //                    client.DownloadDone -= answerRecieved;
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Download done!");

        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Verifying version with server");
        //                    client.VersionVerificationDone += answerRecieved;
        //                    next = false;
        //                    client.RequestVerifyVersion(localVersion.VersionName);
        //                    Console.WriteLine(ConsoleExtension.AddTimestamp("Waiting for answer from server");
        //                    while (!next)
        //                    {

        //                    }
        //                    client.VersionVerificationDone -= answerRecieved;
        //                    if (client.GetVersionModel(localVersion.VersionName).Verified)
        //                    {
        //                        Console.WriteLine(ConsoleExtension.AddTimestamp(localVersion.VersionName + " was VERIFIED by the server, patching DONE!");
        //                        Console.WriteLine(ConsoleExtension.AddTimestamp("Press any key to continue...");
        //                        Console.ReadKey();
        //                        return;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine(ConsoleExtension.AddTimestamp("Bohoo, didn't work");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                client.MissingFileListReceived += answerRecieved;
        //                next = false;
        //                client.RequestVersionMissingFiles(client.serverVersions[choice]);
        //                while (!next) { }
        //                client.MissingFileListReceived -= answerRecieved;

        //                client.DownloadDone += answerRecieved;
        //                next = false;
        //                client.DownloadMissingFiles(client.GetVersionModel(client.serverVersions[choice]));
        //                while (!next) { }
        //                client.DownloadDone -= answerRecieved;

        //                Console.WriteLine(ConsoleExtension.AddTimestamp("Verifying version with server");
        //                client.VersionVerificationDone += answerRecieved;
        //                next = false;
        //                client.RequestVerifyVersion(client.serverVersions[choice]);
        //                Console.WriteLine(ConsoleExtension.AddTimestamp("Waiting for answer from server");
        //                while (!next)
        //                {

        //                }
        //                client.VersionVerificationDone -= answerRecieved;

        //            }


        //        }

        //        static void PatchClientInfo(PatchClient client)
        //        {
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Information stored in patchclient:");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Versions available on server: " + client.serverVersions.Length);
        //            for (int i = 0; i < client.serverVersions.Length; i++)
        //            {
        //                Console.WriteLine(ConsoleExtension.AddTimestamp(client.serverVersions[i]);
        //            }
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("-----------------------------------");
        //            Console.WriteLine(ConsoleExtension.AddTimestamp("Locally found versions:" + client.InstalledVersions.Count);
        //            foreach (var item in client.InstalledVersions)
        //            {
        //                Console.Write(item.VersionName);
        //                Console.Write(" - ");
        //                if (item.Verified)
        //                {
        //                    Console.ForegroundColor = ConsoleColor.Green;
        //                    Console.Write("VERIFIED\n");
        //                }
        //                else
        //                {
        //                    Console.ForegroundColor = ConsoleColor.Red;
        //                    Console.Write("NOT VERIFIED\n");
        //                }
        //                Console.ForegroundColor = ConsoleColor.White;
        //            }
        //        }
    }
}
