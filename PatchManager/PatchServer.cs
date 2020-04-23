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
using System.Linq;
using System.IO;
using System.Threading;
using HelperTools;

namespace PatchManager {
    public class PatchServer {
        List<TcpClient> clients = new List<TcpClient>();
        //Dictionary<string, Dictionary<string, string>> masterFiles;
        List<InstallationDataModel> masterFiles = new List<InstallationDataModel>();
        string masterDirectory = "TestFiles";
        string [ ] versions = new string [ 0 ];
        bool running;

        public PatchServer () {
        }

        ~PatchServer () {
            running = false;
        }

        public void UpdateMasterFiles () {
            //masterFiles = new Dictionary<string, Dictionary<string, string>>();

            try {
                masterFiles = ChecksumTool.GetInstalledVersions(masterDirectory);
                versions = new string [ masterFiles.Count ];
                for (int i = 0; i < masterFiles.Count; i++) {
                    versions [ i ] = masterFiles [ i ].VersionBranch.ToString();
                }
                //versions = ChecksumTool.GetAvailableFolders(masterDirectory);
                //for (int i = 0; i < versions.Length; i++)
                //{
                //    Console.WriteLine(ConsoleExtension.AddTimestamp("Find files for: " + versions[i]);
                //    Console.WriteLine(ConsoleExtension.AddTimestamp("At path: " + masterDirectory + @"\" + versions[i]);
                //    Dictionary<string, string> temp = new Dictionary<string, string>();
                //    if (masterDirectory != "")
                //        ChecksumTool.GetFilesDictionary(out temp, masterDirectory + @"\" + versions[i]);
                //    else
                //        ChecksumTool.GetFilesDictionary(out temp, versions[i]);
                //    masterFiles.Add(versions[i], temp);
                //}

                //Console.WriteLine(ConsoleExtension.AddTimestamp("Versions was:");
                //for (int i = 0; i < versions.Length; i++)
                //{
                //    Console.WriteLine(ConsoleExtension.AddTimestamp(versions[i]);
                //}
            }
            catch (Exception e) {
                Console.WriteLine(ConsoleExtension.AddTimestamp("Updating master files FAILED!"));
                Console.WriteLine(ConsoleExtension.AddTimestamp(e.Message));
            }
        }

        public void Start () {
            UpdateMasterFiles();
            if (!running)
                Task.Run(() => HandleIncomingConnections());
        }

        public void Stop () {
            running = false;
        }

        public void HandleIncomingConnections () {
            TcpListener listener = new TcpListener(IPAddress.Any, Configs.PATCH_SERVER_PORT);
            listener.Start();
            running = true;
            Console.WriteLine(ConsoleExtension.AddTimestamp("Started handling incoming connections"));
            while (running) {
                TcpClient client = listener.AcceptTcpClient();

                lock (clients) {
                    Console.WriteLine(ConsoleExtension.AddTimestamp("New connection added!"));
                    clients.Add(client);
                    Task.Run(() => HandleClientConnection(client));
                }
            }
            Console.WriteLine(ConsoleExtension.AddTimestamp("Stopped handling incoming connections"));
        }

        public void HandleClientConnection (TcpClient client) {
            //While the client is still connected, await communication
            while (ConnectionHandler.Connected(client)) {
                //Wait for data from client
                if (client.GetStream().DataAvailable) {
                    Console.WriteLine(ConsoleExtension.AddTimestamp("Recieving request"));
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(client.GetStream()));

                    switch (data.RequestType) {
                        case PatchNetworkRequest.AvailableVersions:
                            SendAvailableVersions(client);
                            Console.WriteLine(ConsoleExtension.AddTimestamp("Versions send to client"));
                            break;
                        case PatchNetworkRequest.VerifyVersion:
                            SendVersionVerification(client, data);
                            break;
                        case PatchNetworkRequest.MissingFiles:
                            SendMissingFilesList(client, data);
                            break;
                        case PatchNetworkRequest.DownloadFile:
                            SendFileToClient(client, data);
                            break;
                        case PatchNetworkRequest.TestConnection:
                            SendTestConnectionResponse(client);
                            Console.WriteLine(ConsoleExtension.AddTimestamp("Send test response to client"));
                            break;
                    }
                }

                Thread.Sleep(30);
            }

            lock (clients) {
                clients.Remove(client);
                Console.WriteLine(ConsoleExtension.AddTimestamp("Client disconnected"));
            }
        }

        private void SendTestConnectionResponse (TcpClient client) {
            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.TestConnection,
                Connected = true,
                Message = "You're in!"
            };
            byte [ ] data = ConnectionHandler.ConvertToBytes<PatchDataModel>(model);
            client.GetStream().Write(data, 0, data.Length);
        }

        private void SendAvailableVersions (TcpClient client) {
            PatchDataModel model = new PatchDataModel() {
                RequestType = PatchNetworkRequest.AvailableVersions,
                AvailableBranches = versions
            };
            byte [ ] data = ConnectionHandler.ConvertToBytes<PatchDataModel>(model);
            client.GetStream().Write(data, 0, data.Length);
        }

        private void SendVersionVerification (TcpClient client, PatchDataModel data) {
            //Check if the versions combined checksum matches that of the server
            var temp = masterFiles.FirstOrDefault(x => x.VersionBranch == data.InstalledVersion.VersionBranch);
            if (temp != null) {
                if (temp.InstallationChecksum == data.InstalledVersion.InstallationChecksum) {
                    data.InstalledVersion.Status = InstallationStatus.Verified;
                    ConnectionHandler.SendObject(data, client);
                    Console.WriteLine(ConsoleExtension.AddTimestamp("Installation checksum matched and " + data.InstalledVersion.VersionName + " was VERIFIED"));
                    return;
                }
                else {
                    data.InstalledVersion.Status = InstallationStatus.UpdateRequired;
                    Console.WriteLine(ConsoleExtension.AddTimestamp("Installation checksum did NOT match and " + data.InstalledVersion.VersionName + " was NOT VERIFIED"));
                    ConnectionHandler.SendObject(data, client);
                }
            }
            else {
                Console.WriteLine(ConsoleExtension.AddTimestamp("Version: " + data.InstalledVersion.VersionName + " NOT FOUND"));
                ConnectionHandler.SendObject(data, client);
                return;
            }


        }

        private void SendMissingFilesList (TcpClient client, PatchDataModel data) {
            //Find the requested version
            var temp = masterFiles.FirstOrDefault(x => x.VersionBranch == data.InstalledVersion.VersionBranch);
            List<string> missingFiles = new List<string>();
            if (temp != null) {
                Console.WriteLine(ConsoleExtension.AddTimestamp("List found"));
                //Check which files are missing/mismatched
                missingFiles = ChecksumTool.CompareFileDictionaries(temp.GetFilesAsDictionary(), data.InstalledVersion.GetFilesAsDictionary());
                Console.WriteLine(ConsoleExtension.AddTimestamp("Missing files found: " + missingFiles.Count));
                string dir = ChecksumTool.RootedPathCheck(masterDirectory + '/' + temp.VersionBranch.ToString());
                Console.WriteLine(ConsoleExtension.AddTimestamp("Dir: " + dir));
                data.InstalledVersion = GenerateInstallationDataModel(missingFiles, dir);

                data.InstalledVersion.VersionName = temp.VersionName;
                Console.WriteLine(ConsoleExtension.AddTimestamp("Installation datamodel created: " + data.InstalledVersion.VersionName));
                ConnectionHandler.SendObject(data, client);
                return;
            }

            data.InstalledVersion.Files.Clear();
            data.Message = "Version doesn't exist on server";
            ConnectionHandler.SendObject(data, client);
            Console.WriteLine(ConsoleExtension.AddTimestamp("Requested version does not exist on the server!"));
        }

        private InstallationDataModel GenerateInstallationDataModel (List<string> filesToAdd, string directory = "") {
            InstallationDataModel model = new InstallationDataModel();

            foreach (var item in filesToAdd) {
                string fileName = directory + '/' + item;
                Console.WriteLine(ConsoleExtension.AddTimestamp("Filename: " + fileName));
                FileInfo t = new FileInfo(fileName);
                model.MissingFiles.Add(new FileModel() { FilePath = item, Size = t.Length });
                model.TotalSize += t.Length;
            }

            model.RemainingSize += model.TotalSize;
            return model;
        }

        private void SendFileToClient (TcpClient client, PatchDataModel data) {
            var temp = masterFiles.FirstOrDefault(x => x.VersionBranch == data.InstalledVersion.VersionBranch);
            if (temp != null) {
                string path = "";
                if (data.InstalledVersion.VersionName != String.Empty)
                    path = masterDirectory + '/' + data.InstalledVersion.VersionName + '/' + data.File.FilePath;
                else
                    path = masterDirectory + '/' + data.File.FilePath;

                FileInfo fi = new FileInfo(path);
                Console.WriteLine(ConsoleExtension.AddTimestamp($"{fi.Name} size: {fi.Length}"));
                byte [ ] preBuffer = BitConverter.GetBytes((int)fi.Length);
                client.Client.SendFile(fi.FullName, preBuffer, null, TransmitFileOptions.UseDefaultWorkerThread);
                Console.WriteLine(ConsoleExtension.AddTimestamp($"{fi.Name} sent"));
            }
            else
                Console.WriteLine(ConsoleExtension.AddTimestamp($"{data.File.FilePath} was not found under version: {data.InstalledVersion.VersionName}"));
        }
    }
}
