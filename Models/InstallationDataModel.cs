using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Models {
    public enum InstallationStatus { Unchecked, Verified, NotInstalled, UpdateRequired, NotFoundOnServer, IsInstalling, IsDeleting }
    public enum VersionBranch { None, Release, Beta, Development }

    public class InstallationDataModel {
        public string VersionName { get; set; }
        public VersionBranch VersionBranch;
        public string InstallationChecksum { get; set; }
        public List<FileModel> Files { get; set; } = new List<FileModel>();
        public List<FileModel> MissingFiles { get; set; } = new List<FileModel>();
        public long TotalSize { get; set; }
        public long RemainingSize { get; set; }
        public InstallationStatus Status { get; set; }
        public string InstallPath { get; set; }

        public string VersionBranchToString {
            get {
                return VersionBranch.ToString();
            }
        }


        [JsonIgnore]
        private Dictionary<string, string> FilesDictionary = null;
        public Dictionary<string, string> GetFilesAsDictionary () {
            if (FilesDictionary == null) {
                FilesDictionary = new Dictionary<string, string>();

                foreach (var item in Files) {
                    FilesDictionary.Add(item.FilePath, item.FileChecksum);
                }
            }

            return FilesDictionary;
        }

        public void SaveToFile () {
            File.WriteAllText(@InstallPath + @"/Version.json", JsonConvert.SerializeObject(this));
        }

        public bool LoadFromFile () {
            if (File.Exists(@InstallPath + @"/Version.json")) {
                var temp = JsonConvert.DeserializeObject<InstallationDataModel>(File.ReadAllText(@InstallPath + @"/Version.json"));
                this.Files = temp.Files;
                this.InstallationChecksum = temp.InstallationChecksum;
                this.MissingFiles = temp.MissingFiles;
                this.TotalSize = temp.TotalSize;
                this.RemainingSize = temp.RemainingSize;
                this.Status = temp.Status;
                this.VersionBranch = temp.VersionBranch;
                this.VersionName = temp.VersionName;

                return true;
            }
            return false;
        }

        public static InstallationDataModel GetModelFromFile (string pathToFolder) {
            if (File.Exists(@pathToFolder + @"/Version.json")) {
                return JsonConvert.DeserializeObject<InstallationDataModel>(File.ReadAllText(@pathToFolder + @"/Version.json"));
            }
            return null;
        }
    }

    public class FileModel {
        public string FileChecksum { get; set; }
        public string FilePath { get; set; }
        public long Size { get; set; }
    }
}
