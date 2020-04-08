using System;

namespace Models
{
    public enum PatchNetworkRequest { AvailableVersions, CheckFiles, TestConnection }
    public class PatchDataModel
    {
        public PatchNetworkRequest RequestType;
        public bool Connected;
        public string TestString;
        public string[] Versions;
    }
}
