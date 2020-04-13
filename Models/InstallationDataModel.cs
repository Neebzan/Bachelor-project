using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class InstallationDataModel
    {
        public string VersionName { get; set; }
        public string InstallationChecksum { get; set; }
        public List<FileModel> Files { get; set; } = new List<FileModel>();
        public List<FileModel> MissingFiles { get; set; } = new List<FileModel>();
        public long TotalSize { get; set; }
        public long RemainingSize { get; set; }
        public bool Verified { get; set; }
        public string InstallPath { get; set; }

        [JsonIgnore]
        private Dictionary<string,string> FilesDictionary = null;
        public Dictionary<string, string> GetFilesAsDictionary()
        {
            if(FilesDictionary == null)
            {
                FilesDictionary = new Dictionary<string, string>();

                foreach (var item in Files)
                {
                    FilesDictionary.Add(item.FilePath, item.FileChecksum);
                }
            }

            return FilesDictionary;
        }
    }

    public class FileModel
    {
        public string FileChecksum { get; set; }
        public string FilePath { get; set; }
        public long Size { get; set; }
    }
}
