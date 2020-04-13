using System;

namespace Models
{
    public enum PatchNetworkRequest { AvailableVersions, VerifyVersion, TestConnection, DownloadFile, MissingFiles }
    public class PatchDataModel
    {
        public PatchNetworkRequest RequestType;
        public InstallationDataModel InstalledVersion;
        public bool Connected;
        public FileModel File;
        public string Message;
        public string[] Versions;
    }
}
