using ChecksumHandlerLib;
using ConnectionHandlerLib;
using GlobalConfigs;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatchClientLib
{
    public class PatchClient
    {
        TcpClient client;

        string ip;
        int port;

        public string[] serverVersions = new string[0];
        //public Dictionary<string, Dictionary<string, string>> InstalledVersions = new Dictionary<string, Dictionary<string, string>>();
        public List<InstallationDataModel> InstalledVersions = new List<InstallationDataModel>();
        string installPath;

        bool running;
        bool downloadingFiles;

        public event Action VersionVerificationDone;
        public event Action MissingFileListReceived;
        public event Action VersionsFromServerReceived;
        public event Action DownloadDone;


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

        public void DisconnectFromServer()
        {
            running = false;
        }

        public void UpdateCurrentInstallations()
        {
            InstalledVersions = ChecksumTool.GetInstalledVersions(installPath);
        }

        public void HandleResponse()
        {
            running = true;
            while (ConnectionHandler.Connected(client) && running)
            {
                if (!downloadingFiles)
                    if (client.GetStream().DataAvailable)
                    {
                        Console.WriteLine("Incoming response");
                        PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));

                        switch (data.RequestType)
                        {
                            case PatchNetworkRequest.AvailableVersions:
                                HandleAvailableVersionsResponse(data);
                                break;
                            case PatchNetworkRequest.VerifyVersion:
                                HandleVerifyVersionResponse(data);
                                break;
                            case PatchNetworkRequest.MissingFiles:
                                HandleVersionMissingFilesResponse(data);
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
            Console.WriteLine(data.Message);
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
            VersionsFromServerReceived?.Invoke();
        }

        public void RequestVerifyVersions()
        {
            foreach (var item in InstalledVersions)
            {
                RequestVerifyVersion(item);
            }
        }

        public void RequestVerifyVersion(InstallationDataModel versionData)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.VerifyVersion,
                InstalledVersion = new InstallationDataModel()
                {
                    VersionName = versionData.VersionName,
                    InstallationChecksum = versionData.InstallationChecksum
                }
            };
            ConnectionHandler.SendObject(model, client);
            Console.WriteLine(versionData.VersionName + " send to server for verification");
        }

        public void RequestVerifyVersion(string versionName)
        {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null)
            {
                RequestVerifyVersion(temp);
            }
        }

        private void HandleVerifyVersionResponse(PatchDataModel data)
        {
            if (data.InstalledVersion.Verified)
            {
                var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == data.InstalledVersion.VersionName);
                if (temp != null)
                {
                    temp.Verified = true;
                    Console.WriteLine(temp.VersionName + " was verified by server");                    
                }
            }
            else
                Console.WriteLine(data.InstalledVersion.VersionName + " was NOT verified by server");

            VersionVerificationDone?.Invoke();
        }

        public void RequestVersionMissingFiles(string versionName)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.MissingFiles,
            };

            //Check if the requested version exists locally
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null)
            {
                model.InstalledVersion = temp;
            }
            else
                model.InstalledVersion = new InstallationDataModel()
                {
                    VersionName = versionName
                };

            ConnectionHandler.SendObject(model, client);

            //Console.WriteLine(versionData.VersionName + " send to server for verification");
        }
        private void HandleVersionMissingFilesResponse(PatchDataModel data)
        {
            //Find the requested version
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == data.InstalledVersion.VersionName);
            if (temp != null)
            {
                temp.MissingFiles = data.InstalledVersion.MissingFiles;

                //TODO
                foreach (var item in temp.MissingFiles)
                {
                    Console.WriteLine("MISSING FILE: " + item.FilePath);
                }
            }
            else
            {
                InstalledVersions.Add(data.InstalledVersion);
            }

            MissingFileListReceived?.Invoke();
        }

        public void DownloadMissingFiles(InstallationDataModel version)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.DownloadFile,
                InstalledVersion = new InstallationDataModel
                {
                    VersionName = version.VersionName
                }
            };
            downloadingFiles = true;
            //While there's still files missing and there's still an active connection
            while (version.MissingFiles.Count > 0 && ConnectionHandler.Connected(client))
            {
                //Request file
                model.File = new FileModel()
                {
                    FilePath = version.MissingFiles[0].FilePath
                };
                ConnectionHandler.SendObject(model, client);

                //Wait for response
                while (!client.GetStream().DataAvailable)
                {
                    Thread.Sleep(16);
                }

                //Handle incoming file
                ConnectionHandler.ReadFile(client, version.MissingFiles[0].FilePath, installPath + '/' + version.VersionName);
                Console.WriteLine(version.MissingFiles[0].FilePath + " downloaded");
                version.MissingFiles.RemoveAt(0);
            }
            downloadingFiles = false;
            Console.WriteLine("All missing files received!");
            DownloadDone?.Invoke();
            UpdateCurrentInstallations();
            //RequestVerifyVersion(version);
        }

        public bool CheckIfVersionExists(string versionName)
        {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null)
            {
                return true;
            }
            return false;
        }

        public InstallationDataModel GetVersionModel(string versionName)
        {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null)
            {
                return temp;
            }
            return null;
        }
    }
}
