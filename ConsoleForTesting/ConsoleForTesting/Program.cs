using ChecksumHandlerLib;
using ConnectionHandlerLib;
using DatabaseLib;
using GlobalConfigs;
using Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PatchClientLib;
using System;
using System.Collections.Generic;
using System.Data;
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

            //PatchClientTest();
            //string test = ChecksumTool.GetCombinedChecksum("ClientFiles");

            //DBConnection.Instance().Password = "password";
            //DBConnection.Instance().Username = "root";
            //DBConnection.Instance().ServerIP = "localhost";
            //DBConnection.Instance().ServerPort = 3306;
            //DBConnection.Instance().DatabaseName = "intrusive";





            //var t = DBConnection.Instance().GetAllPlayerMatches("Bleh3");

            //var b = DBConnection.Instance().GetAllTest("Bleh3");


            //DBConnection.Instance().Insert(testModel);
            //.Instance().Insert(testModel);
            //DBConnection.Instance().GetAll<AccountModel>();
            //var testGet = DBConnection.Instance().Get<AccountModel>("Bleh3");


            //DBTests();
            PatchClientTest();

            Console.ReadKey();
        }

        static void DBTests()
        {
            AccountModel testAccountModel = new AccountModel()
            {
                account_id = "Bleh3",
                first_name = "Mr Bleh2",
                last_name = "Sausage2",
                email = "yes@no.com2",
                password_hash = "Very secure PW2"
            };

            AccountModel testAccountModel2 = new AccountModel()
            {
                account_id = "Bleh1",
                first_name = "Mr Bleh2",
                last_name = "Sausage2",
                email = "yes@no.com2",
                password_hash = "Very secure PW2"
            };

            PlayerModel testPlayer = new PlayerModel()
            {
                player_id = "Bleh3"
            };

            PlayerModel testPlayer2 = new PlayerModel()
            {
                player_id = "Bleh1"
            };

            MatchModel testMatch = new MatchModel()
            {
                map_name = "map1",
                begun = DateTime.Now,
                ended = DateTime.Now.AddDays(1),
                difficulty = 100
            };

            MatchModel testMatch2 = new MatchModel()
            {
                map_name = "map2",
                begun = DateTime.Now,
                ended = DateTime.Now.AddDays(1),
                difficulty = 100
            };

            MapModel testMap = new MapModel()
            {
                map_name = "map1"
            };

            MapModel testMap2 = new MapModel()
            {
                map_name = "map2"
            };

            PlayedMatch testPlayedMatch = new PlayedMatch()
            {
                deaths = 5,
                score = 999,
                kills = 10,
                match_id = 1,
                player_id = "Bleh3"
            };

            ItemColor testColor = new ItemColor()
            {
                color_name = "blue",
                red = 0,
                green = 0,
                blue = 255
            };

            ItemType itemTypeTest = new ItemType()
            {
                type_value = "test"
            };

            ItemModel testItemModel = new ItemModel()
            {
                aquire_date = DateTime.Now,
                item_color = "blue",
                item_name = "testItem",
                item_type = "test",
                owner_id = "Bleh3",
                quality = 0.5f
            };

            AbilityModel testAbility1 = new AbilityModel()
            {
                ability_name = "Test Firebawl",
                cost = 2
            };

            AbilityModel testAbility2 = new AbilityModel()
            {
                ability_name = "Test FrostBawl",
                cost = 3
            };

            UnlockedAbility testUnlockedAbility1 = new UnlockedAbility()
            {
                ability_name = "Test Firebawl",
                player_id = "Bleh3"
            };

            UnlockedAbility testUnlockedAbility2 = new UnlockedAbility()
            {
                ability_name = "Test Frostbawl",
                player_id = "Bleh3"
            };

            Wears wearTest = new Wears()
            {
                player_id = "Bleh3",
                item_id = 2
            };

            //ItemModel t = DBConnection.Instance().Get<ItemModel>(1);
            //DBConnection.Instance().Insert(testAccountModel);
            //DBConnection.Instance().Insert(testPlayer);
            //DBConnection.Instance().Insert(testAccountModel2);
            //DBConnection.Instance().Insert(testPlayer2);
            //DBConnection.Instance().Insert(testColor);
            //DBConnection.Instance().Insert(testMap);
            //DBConnection.Instance().Insert(itemTypeTest);
            //DBConnection.Instance().Insert(testItemModel);
            //DBConnection.Instance().Insert(testMatch);
            //DBConnection.Instance().Insert(testMatch2);
            //DBConnection.Instance().Insert(testPlayedMatch);
            //DBConnection.Instance().Insert(wearTest);
            //DBConnection.Instance().Insert(testAbility1);
            //DBConnection.Instance().Insert(testAbility2);
            //DBConnection.Instance().Insert(testUnlockedAbility1);
            //DBConnection.Instance().Insert(testUnlockedAbility2);

            //var matches = DBConnection.Instance().GetPlayerMatches(testPlayer.player_id);

        }

        static void PatchClientTest()
        {
            bool running = true;
            PatchClient pClient = new PatchClient(13000, "212.10.51.254");
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
                    case ConsoleKey.D4:
                        WriteCommands(pClient);
                        pClient.RequestVerifyVersions();
                        break;
                    case ConsoleKey.D5:
                        WriteCommands(pClient);
                        pClient.RequestVersionMissingFiles("VersionTest1");
                        break;
                    case ConsoleKey.D6:
                        WriteCommands(pClient);
                        pClient.DownloadMissingFiles(pClient.InstalledVersions.FirstOrDefault(x => x.VersionName == "VersionTest1"));
                        break;
                    case ConsoleKey.D7:
                        WriteCommands(pClient);
                        FullPatch(pClient);
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
            Console.WriteLine("4. Verify versions with server");
            Console.WriteLine("5. Get missing files from server (VersionTest1)");
            Console.WriteLine("6. Download missing files from server (VersionTest1)");
            Console.WriteLine("7. Run full patch");


            Console.WriteLine("\n----------------------------------\n");
            PatchClientInfo(pClient);


            Console.WriteLine("\n----------------------------------\n");
            Console.WriteLine("Log:\n");
        }

        static void FullPatch(PatchClient client)
        {
            bool next = false;
            Action answerRecieved = new Action(() =>
            {
                next = true;
            });

            client.UpdateCurrentInstallations();
            Console.Clear();
            ConsoleKeyInfo key = new ConsoleKeyInfo();

            client.VersionsFromServerReceived += answerRecieved;
            Console.WriteLine("Requesting versions from server!");
            client.RequestAvailableVersions();
            while (!next)
            {

            }
            client.VersionsFromServerReceived -= answerRecieved;
            next = false;
            Console.WriteLine("Choose which version to patch");
            for (int i = 0; i < client.serverVersions.Length; i++)
            {
                Console.WriteLine(i + 1 + ". " + client.serverVersions[i]);
            }
            int choice = -1;
            while (!next)
            {
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        choice = 0;
                        next = true;
                        break;
                    case ConsoleKey.D2:
                        choice = 1;
                        next = true;
                        break;
                }
            }

            next = false;

            Console.WriteLine("Attempting to verify current intallation of: " + client.serverVersions[choice]);
            if (client.CheckIfVersionExists(client.serverVersions[choice]))
            {
                var localVersion = client.GetVersionModel(client.serverVersions[choice]);
                Console.WriteLine("A version of {0} exists locally", client.serverVersions[choice]);
                Console.WriteLine("Verifying version with server");

                client.VersionVerificationDone += answerRecieved;
                client.RequestVerifyVersion(client.serverVersions[choice]);
                Console.WriteLine("Waiting for answer from server");
                while (!next)
                {

                }
                client.VersionVerificationDone -= answerRecieved;

                if (localVersion.Verified)
                {
                    Console.WriteLine(localVersion.VersionName + " was VERIFIED by the server, patching DONE!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    Console.WriteLine(localVersion.VersionName + " was NOT VERIFIED by the server, patching DONE!");
                    Console.WriteLine("Requesting missing/broken files");
                    client.MissingFileListReceived += answerRecieved;
                    next = false;

                    client.RequestVersionMissingFiles(localVersion.VersionName);
                    while (!next)
                    {

                    }
                    client.MissingFileListReceived -= answerRecieved;
                    Console.WriteLine("Missing files recieved!");
                    Console.WriteLine("Attempting to download missing files");

                    client.DownloadDone += answerRecieved;
                    next = false;
                    client.DownloadMissingFiles(localVersion);
                    while (!next)
                    {

                    }
                    client.DownloadDone -= answerRecieved;
                    Console.WriteLine("Download done!");

                    Console.WriteLine("Verifying version with server");
                    client.VersionVerificationDone += answerRecieved;
                    next = false;
                    client.RequestVerifyVersion(localVersion.VersionName);
                    Console.WriteLine("Waiting for answer from server");
                    while (!next)
                    {

                    }
                    client.VersionVerificationDone -= answerRecieved;
                    if (client.GetVersionModel(localVersion.VersionName).Verified)
                    {
                        Console.WriteLine(localVersion.VersionName + " was VERIFIED by the server, patching DONE!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Bohoo, didn't work");
                    }
                }
            }
            else
            {
                client.MissingFileListReceived += answerRecieved;
                next = false;
                client.RequestVersionMissingFiles(client.serverVersions[choice]);
                while (!next) { }
                client.MissingFileListReceived -= answerRecieved;

                client.DownloadDone += answerRecieved;
                next = false;
                client.DownloadMissingFiles(client.GetVersionModel(client.serverVersions[choice]));
                while (!next) { }
                client.DownloadDone -= answerRecieved;

                Console.WriteLine("Verifying version with server");
                client.VersionVerificationDone += answerRecieved;
                next = false;
                client.RequestVerifyVersion(client.serverVersions[choice]);
                Console.WriteLine("Waiting for answer from server");
                while (!next)
                {

                }
                client.VersionVerificationDone -= answerRecieved;

            }


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
                Console.Write(item.VersionName);
                Console.Write(" - ");
                if (item.Verified)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("VERIFIED\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("NOT VERIFIED\n");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
