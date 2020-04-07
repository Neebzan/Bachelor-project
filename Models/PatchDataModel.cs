using System;

namespace Models
{
    public enum PatchNetworkRequest { AvailableVersions, CheckFiles }
    public class PatchDataModel
    {
        public PatchNetworkRequest RequestType;
        public string[] Versions;
    }
}
