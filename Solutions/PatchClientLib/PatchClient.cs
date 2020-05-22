using ChecksumHandlerLib;
using ConnectionHandlerLib;
using GlobalConfigs;
using HelperTools;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatchClientLib
{
    public class VersionsFromServerRecievedEventArgs : EventArgs
    {
        public VersionsFromServerRecievedEventArgs(string[] versions)
        {
            Versions = versions;
        }
        public string[] Versions { get; set; }
    }

    public class DownloadProgressEventArgs : EventArgs
    {
        public long TotalSize { get; set; }
        public long DownloadedTotal { get; set; }
        public string NextFileName { get; set; }
        public VersionBranch Version { get; set; }
    }

    public static class PatchClient
    {
        public static event EventHandler<DownloadProgressEventArgs> GetDownloadProgress = delegate { };
        private static void OnDownloadProgress(DownloadProgressEventArgs e)
        {
            var handler = GetDownloadProgress;
            handler(null, e);
        }

        public static TcpClient _client;

        public static string _ip;
        public static int _port;

        public static string InstallPath { get; set; }


        static bool _running;
        static bool _downloadingFiles;

        public static string[] serverBranches = new string[0];
        public static event Action VersionVerificationDone;
        public static event Action<string> MissingFileListReceived;
        public static event Action<VersionsFromServerRecievedEventArgs> VersionsFromServerReceived;
        public static event Action<InstallationDataModel> DownloadDone;

        public static List<InstallationDataModel> InstalledVersions = new List<InstallationDataModel>();

        static PatchClient()
        {
            //ConnectToServer(30831, "212.10.51.254");
            _ip = "212.10.51.254";
            //_ip = "localhost";
            _port = 30004;
            //_port = 13000;

        }

        public static void SetConnectionInfo(int port, string ip)
        {
            _ip = ip;
            _port = port;
        }

        public static void DisconnectFromServer()
        {
            _running = false;
        }

        public static List<InstallationDataModel> CompleteCheck(string[] paths)
        {
            //Update local installations
            for (int i = 0; i < paths.Length; i++)
            {
                InstalledVersions.Add(ChecksumTool.GetInstalledVersion(paths[i]));
            }

            //Establish connection to server
            _client = new TcpClient(_ip, _port);
            if (ConnectionHandler.Connected(_client))
            {
                InstalledVersions = CompareLocalVersionsToServerVersions(InstalledVersions);
            }

            _client.GetStream().Close();
            _client.Close();

            return InstalledVersions;
        }

        public static async Task<List<InstallationDataModel>> CompleteCheckAsync(string[] paths)
        {

            //Update local installations
            for (int i = 0; i < paths.Length; i++)
            {
                InstalledVersions.Add(ChecksumTool.GetInstalledVersion(paths[i]));
            }

            //Establish connection to server
            _client = new TcpClient(_ip, _port);
            if (ConnectionHandler.Connected(_client))
            {
                await CompareLocalVersionsToServerVersionsAsync();
            }

            _client.GetStream().Close();
            _client.Close();

            return InstalledVersions;
        }


        /// <summary>
        /// Checks and attemps to verify currently detected local versions with the patch server, and checks for missing files.
        /// Also adds versions that still doesn't exist locally as empty installation models
        /// </summary>
        private static List<InstallationDataModel> CompareLocalVersionsToServerVersions(List<InstallationDataModel> installations)
        {
            //Get versions from server
            serverBranches = RequestAvailableVersions();

            //List of versions that exists both locally and on server
            List<string> matchingBranches = new List<string>();

            for (int i = 0; i < installations.Count; i++)
            {
                if (serverBranches.Contains(installations[i].VersionBranchToString))
                {
                    installations[i] = CompareLocalVersionWithServer(installations[i]);
                    matchingBranches.Add(installations[i].VersionBranchToString);
                }
                else
                {
                    installations[i].Status = InstallationStatus.NotFoundOnServer;
                }
            }


            //Find server versions that wasen't found locally and create empty installation model for them
            foreach (var branch in serverBranches)
            {
                if (!matchingBranches.Contains(branch))
                {
                    InstallationDataModel temp = new InstallationDataModel()
                    {
                        VersionBranch = (VersionBranch)Enum.Parse(typeof(VersionBranch), branch),
                        Status = InstallationStatus.NotInstalled
                    };

                    temp = CompareLocalVersionWithServer(temp);
                    installations.Add(temp);
                }
            }

            return installations;
        }

        private static async Task CompareLocalVersionsToServerVersionsAsync()
        {
            //Get versions from server
            serverBranches = RequestAvailableVersions();

            //List of versions that exists both locally and on server
            List<string> matchingVersions = new List<string>();

            foreach (var curVersion in InstalledVersions)
            {
                if (serverBranches.Contains(curVersion.VersionName))
                {
                    await CompareLocalVersionWithServerAsync(curVersion);
                    matchingVersions.Add(curVersion.VersionName);
                }
                else
                {
                    curVersion.Status = InstallationStatus.NotFoundOnServer;
                }
            }

            //Find server versions that wasen't found locally and create empty installation model for them
            foreach (var version in serverBranches)
            {
                if (!matchingVersions.Contains(version))
                {
                    InstallationDataModel temp = new InstallationDataModel()
                    {
                        VersionName = version,
                        Status = InstallationStatus.NotInstalled
                    };

                    InstalledVersions.Add(temp);
                    await CompareLocalVersionWithServerAsync(temp);
                }
            }

        }


        /// <summary>
        /// Attempts to verify an installation with the patch server, if it fails, attempt to get missing files
        /// </summary>
        /// <param name="version"></param>
        public static InstallationDataModel CompareLocalVersionWithServer(InstallationDataModel version)
        {
            RequestVerifyVersion(ref version);
            if (version.Status != InstallationStatus.Verified)
            {
                RequestVersionMissingFiles(ref version);
            }
            return version;
        }

        public static async Task CompareLocalVersionWithServerAsync(InstallationDataModel version)
        {
            RequestVerifyVersion(ref version);
            if (version.Status != InstallationStatus.Verified)
                await RequestVersionMissingFilesAsync(version);
        }

        /// <summary>
        /// Generates model data for each version installed on the path
        /// </summary>
        //public static void UpdateCurrentInstallations () {
        //    InstalledVersions = ChecksumTool.GetInstalledVersions(InstallPath);
        //}

        /// <summary>
        /// Listens for responses from the patching service after connecting
        /// </summary>
        private static PatchDataModel ReadServerResponse()
        {
            while (ConnectionHandler.Connected(_client))
            {
                if (_client.GetStream().DataAvailable)
                {
                    PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(ConnectionHandler.ReadMessage(_client.GetStream()));
                    return data;
                }
                Thread.Sleep(16);
            }
            return null;
        }

        private static async Task<PatchDataModel> ReadServerResponseAsync()
        {
            PatchDataModel data = JsonConvert.DeserializeObject<PatchDataModel>(await ConnectionHandler.ReadMessageAsync(_client.GetStream()));
            return data;
        }

        public static string[] RequestAvailableVersions()
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.AvailableVersions
            };
            ConnectionHandler.SendObject(model, _client);
            Console.WriteLine(ConsoleExtension.AddTimestamp("Version request send to server"));

            PatchDataModel response = ReadServerResponse();

            if (response != null)
                return response.AvailableBranches;
            else
                return new string[0];

        }

        //public static void RequestVerifyVersions()
        //{
        //    foreach (var item in InstalledVersions)
        //    {
        //        RequestVerifyVersion(ref item);
        //    }
        //}

        public static bool RequestVerifyVersion(ref InstallationDataModel versionData)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.VerifyVersion,
                InstalledVersion = new InstallationDataModel()
                {
                    VersionName = versionData.VersionName,
                    VersionBranch = versionData.VersionBranch,
                    InstallationChecksum = versionData.InstallationChecksum
                }
            };
            ConnectionHandler.SendObject(model, _client);

            PatchDataModel response = ReadServerResponse();

            if (response != null)
            {
                if (response.InstalledVersion.Status == InstallationStatus.Verified)
                {
                    versionData.Status = response.InstalledVersion.Status;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        //public static void RequestVerifyVersion(string versionName)
        //{
        //    var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
        //    if (temp != null)
        //    {
        //        RequestVerifyVersion(temp);
        //    }
        //}

        private static void HandleVerifyVersionResponse(PatchDataModel data)
        {
            if (data.InstalledVersion.Status == InstallationStatus.Verified)
            {
                var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == data.InstalledVersion.VersionName);
                if (temp != null)
                {
                    temp.Status = InstallationStatus.Verified;
                    Console.WriteLine(ConsoleExtension.AddTimestamp(temp.VersionName + " was verified by server"));
                }
            }
            else
                Console.WriteLine(ConsoleExtension.AddTimestamp(data.InstalledVersion.VersionName + " was NOT verified by server"));

            VersionVerificationDone?.Invoke();
        }

        public static List<FileModel> RequestVersionMissingFiles(ref InstallationDataModel version)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.MissingFiles,
                InstalledVersion = version
            };

            ConnectionHandler.SendObject(model, _client);

            PatchDataModel response = ReadServerResponse();

            if (response != null)
            {

                version.MissingFiles = response.InstalledVersion.MissingFiles;
                version.VersionName = response.InstalledVersion.VersionName;
                version.TotalSize = response.InstalledVersion.TotalSize;
                version.RemainingSize = response.InstalledVersion.RemainingSize;

                if (version.MissingFiles.Count > 0 && version.Status != InstallationStatus.NotInstalled)
                    version.Status = InstallationStatus.UpdateRequired;

                return response.InstalledVersion.MissingFiles;
            }
            else
                return null;
        }

        public static async Task<List<FileModel>> RequestVersionMissingFilesAsync(InstallationDataModel version)
        {
            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.MissingFiles,
                InstalledVersion = version
            };

            await ConnectionHandler.SendObjectAsync(model, _client);

            PatchDataModel response = await ReadServerResponseAsync();

            if (response != null)
            {
                version.MissingFiles = response.InstalledVersion.MissingFiles;
                if (version.MissingFiles.Count > 0 && version.Status != InstallationStatus.NotInstalled)
                    version.Status = InstallationStatus.UpdateRequired;

                return response.InstalledVersion.MissingFiles;
            }
            else
                return null;
        }

        public static void DownloadMissingFilesNew(InstallationDataModel version)
        {
            _client = new TcpClient(_ip, _port);

            if (version.MissingFiles.Count == 0)
            {
                version = ChecksumTool.RecheckVersion(version);
                RequestVersionMissingFiles(ref version);
            }

            DownloadProgressEventArgs args = new DownloadProgressEventArgs();

            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.DownloadFiles,
                InstalledVersion = new InstallationDataModel
                {
                    VersionName = version.VersionName,
                    VersionBranch = version.VersionBranch,
                    MissingFiles = version.MissingFiles
                }
            };

            args.Version = version.VersionBranch;
            args.TotalSize = version.TotalSize;

            ConnectionHandler.SendObject(model, _client);


            _downloadingFiles = true;
            //While there's still files missing and there's still an active connection
            while (version.MissingFiles.Count > 0 && ConnectionHandler.Connected(_client))
            {
                if (_client.GetStream().DataAvailable)
                {
                    //Raise download progress event
                    args.NextFileName = version.MissingFiles[0].FilePath;
                    OnDownloadProgress(args);
                    //Handle incoming file
                    // await ConnectionHandler.ReadFileAsync(_client, version.MissingFiles[0].FilePath, InstallPath + '/' + version.VersionName);                
                    ConnectionHandler.ReadFileStream(_client, version.MissingFiles[0].FilePath, version.MissingFiles[0].Size, version.InstallPath);
                    Console.WriteLine(ConsoleExtension.AddTimestamp(version.MissingFiles[0].FilePath + " downloaded"));
                    args.DownloadedTotal += version.MissingFiles[0].Size;
                    lock (version)
                        version.MissingFiles.RemoveAt(0);
                }
            }

            _downloadingFiles = false;
            Console.WriteLine(ConsoleExtension.AddTimestamp("All missing files received!"));
            version = ChecksumTool.RecheckVersion(version);
            RequestVerifyVersion(ref version);
            _client.GetStream().Close();
            _client.Close();
            DownloadDone?.Invoke(version);
        }

        public static void DownloadMissingFiles(InstallationDataModel version)
        {
            _client = new TcpClient(_ip, _port);

            if (version.MissingFiles.Count == 0)
            {
                version = ChecksumTool.RecheckVersion(version);
                RequestVersionMissingFiles(ref version);
            }

            DownloadProgressEventArgs args = new DownloadProgressEventArgs();

            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.DownloadFile,
                InstalledVersion = new InstallationDataModel
                {
                    VersionName = version.VersionName,
                    VersionBranch = version.VersionBranch
                }
            };

            args.Version = version.VersionBranch;
            args.TotalSize = version.TotalSize;
            //foreach (var item in version.MissingFiles) {
            //    args.TotalSize += item.Size;
            //}



            _downloadingFiles = true;
            //While there's still files missing and there's still an active connection
            while (version.MissingFiles.Count > 0 && ConnectionHandler.Connected(_client))
            {
                //Raise download progress event
                args.NextFileName = version.MissingFiles[0].FilePath;
                OnDownloadProgress(args);

                //Request file
                model.File = new FileModel()
                {
                    FilePath = version.MissingFiles[0].FilePath
                };
                ConnectionHandler.SendObject(model, _client);

                //Wait for response


                while (!_client.GetStream().DataAvailable)
                {
                    Thread.Sleep(16);
                }

                //Handle incoming file
                // await ConnectionHandler.ReadFileAsync(_client, version.MissingFiles[0].FilePath, InstallPath + '/' + version.VersionName);                
                ConnectionHandler.ReadFile(_client, version.MissingFiles[0].FilePath, version.InstallPath);
                Console.WriteLine(ConsoleExtension.AddTimestamp(version.MissingFiles[0].FilePath + " downloaded"));
                args.DownloadedTotal += version.MissingFiles[0].Size;
                lock (version)
                    version.MissingFiles.RemoveAt(0);
            }

            _downloadingFiles = false;
            Console.WriteLine(ConsoleExtension.AddTimestamp("All missing files received!"));
            version = ChecksumTool.RecheckVersion(version);
            RequestVerifyVersion(ref version);
            _client.GetStream().Close();
            _client.Close();
            DownloadDone?.Invoke(version);
        }

        public static bool CheckIfVersionExists(string versionName)
        {
            var temp = InstalledVersions.FirstOrDefault(x => x.VersionName == versionName);
            if (temp != null)
            {
                return true;
            }
            return false;
        }

        public static InstallationDataModel GetVersionModel(string versionName)
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