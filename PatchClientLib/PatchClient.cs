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

namespace PatchClientLib {
    public class VersionsFromServerRecievedEventArgs : EventArgs {
        public VersionsFromServerRecievedEventArgs (string [ ] versions) {
            Versions = versions;
        }
        public string [ ] Versions { get; set; }
    }

  


    public static class PatchClient {
        static TcpClient _client;

        static string _ip;

        public static string InstallPath { get; set; }

        static int _port;

        static bool _running;
        static bool _downloadingFiles;

        public static string [ ] serverVersions = new string [ 0 ];
        public static event Action DownloadDone, VersionVerificationDone;
        public static event Action<string> MissingFileListReceived;
        public static event Action<VersionsFromServerRecievedEventArgs> VersionsFromServerReceived;

        public static List<InstallationDataModel> InstalledVersions = new List<InstallationDataModel>();

        static PatchClient () {
            ConnectToServer(30831, "212.10.51.254");
        }

        /// <summary>
        /// Connects to the patching service and starts listening for responeses
        /// </summary>
        /// <param name="port"></param>
        /// <param name="ip"></param>
        public static void ConnectToServer (int port, string ip = "localhost") {
            _client = new TcpClient(ip, port);
            Task.Run(() => HandleResponse());
        }

        public static void DisconnectFromServer () {
            _running = false;
        }

        /// <summary>
        /// Genereates model data for each version installed on the path
        /// </summary>
        public static void UpdateCurrentInstallations () {
            InstalledVersions = ChecksumTool.GetInstalledVersions(InstallPath);
        }

        /// <summary>
        /// Listens for responses from the patching service after connecting
        /// </summary>
        private static void HandleResponse () {
            _running = true;
            while (ConnectionHandler.Connected(_client) && _running) {
                if (!_downloadingFiles)
                    if (_client.GetStream().DataAvailable) {
                        Console.WriteLine("Incoming response");
                        PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(_client.GetStream()));

                        switch (data.RequestType) {
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


        public static void RequestConnectionTest () {
            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.TestConnection
            };
            ConnectionHandler.SendObject(model, _client);
            Console.WriteLine("Connection test send to server");
        }


        private static void HandleConnectionTestResponse (PatchDataModel data) {
            Console.WriteLine("Response recieved");
            Console.WriteLine("Response was;");
            Console.WriteLine(data.Message);
        }

        public static void RequestAvailableVersions () {
            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.AvailableVersions
            };
            ConnectionHandler.SendObject(model, _client);
            Console.WriteLine("Version request send to server");
        }

        private static void HandleAvailableVersionsResponse (PatchDataModel data) {
            serverVersions = data.Versions;
            Console.WriteLine("Response recieved");
            Console.WriteLine("Versions available:");
            for (int i = 0; i < data.Versions.Length; i++) {
                Console.WriteLine(data.Versions [ i ]);
            }
            VersionsFromServerReceived?.Invoke(new VersionsFromServerRecievedEventArgs(serverVersions));
        }

        public static void RequestVerifyVersions () {
            foreach (var item in InstalledVersions) {
                RequestVerifyVersion(item);
            }
        }

        public static void RequestVerifyVersion (InstallationDataModel versionData) {
            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.VerifyVersion,
                InstalledVersion = new InstallationDataModel() {
                    VersionName = versionData.VersionName,
                    InstallationChecksum = versionData.InstallationChecksum
                }
            };
            ConnectionHandler.SendObject(model, _client);
            Console.WriteLine(versionData.VersionName + " send to server for verification");
        }

        public static void RequestVerifyVersion (string versionName) {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null) {
                RequestVerifyVersion(temp);
            }
        }

        private static void HandleVerifyVersionResponse (PatchDataModel data) {
            if (data.InstalledVersion.Verified) {
                var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == data.InstalledVersion.VersionName);
                if (temp != null) {
                    temp.Verified = true;
                    Console.WriteLine(temp.VersionName + " was verified by server");
                }
            }
            else
                Console.WriteLine(data.InstalledVersion.VersionName + " was NOT verified by server");

            VersionVerificationDone?.Invoke();
        }

        public static void RequestVersionMissingFiles (string versionName) {
            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.MissingFiles,
            };

            //Check if the requested version exists locally
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null) {
                model.InstalledVersion = temp;
            }
            else
                model.InstalledVersion = new InstallationDataModel() {
                    VersionName = versionName
                };

            ConnectionHandler.SendObject(model, _client);

            //Console.WriteLine(versionData.VersionName + " send to server for verification");
        }
        private static void HandleVersionMissingFilesResponse (PatchDataModel data) {
            //Find the requested version
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == data.InstalledVersion.VersionName);
            if (temp != null) {
                temp.MissingFiles = data.InstalledVersion.MissingFiles;

                //TODO
                foreach (var item in temp.MissingFiles) {
                    Console.WriteLine("MISSING FILE: " + item.FilePath);
                }
            }
            else {
                InstalledVersions.Add(data.InstalledVersion);
            }

            MissingFileListReceived?.Invoke(data.InstalledVersion.VersionName);
        }

        public static void DownloadMissingFiles (string versionName) {
            var version = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);

            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.DownloadFile,
                InstalledVersion = new InstallationDataModel {
                    VersionName = version.VersionName
                }
            };
            _downloadingFiles = true;
            //While there's still files missing and there's still an active connection
            while (version.MissingFiles.Count > 0 && ConnectionHandler.Connected(_client)) {
                //Request file
                model.File = new FileModel() {
                    FilePath = version.MissingFiles [ 0 ].FilePath
                };
                ConnectionHandler.SendObject(model, _client);

                //Wait for response
                while (!_client.GetStream().DataAvailable) {
                    Thread.Sleep(16);
                }

                //Handle incoming file
                ConnectionHandler.ReadFile(_client, version.MissingFiles [ 0 ].FilePath, InstallPath + '/' + version.VersionName);
                Console.WriteLine(version.MissingFiles [ 0 ].FilePath + " downloaded");
                version.MissingFiles.RemoveAt(0);
            }
            _downloadingFiles = false;
            Console.WriteLine("All missing files received!");
            DownloadDone?.Invoke();
            UpdateCurrentInstallations();
            //RequestVerifyVersion(version);
        }

        public static bool CheckIfVersionExists (string versionName) {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null) {
                return true;
            }
            return false;
        }

        public static InstallationDataModel GetVersionModel (string versionName) {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null) {
                return temp;
            }
            return null;
        }
    }
}
