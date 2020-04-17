using GameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using PatchClientLib;
using System.Threading.Tasks;
using Models;

namespace GameLauncher {
    public static class Logic {
        #region Properties
        private static bool pathSelected;

        //public static bool PathSelected {
        //    get {
        //        //Should run checks on wether or not this is a valid path
        //        return !String.IsNullOrEmpty(GamePaths);
        //    }
        //    set { pathSelected = value; }
        //}

        private static bool gameInstalled;

        //public static bool GameInstalled {
        //    get {
        //        return PathSelected;
        //    }
        //    set { gameInstalled = value; }
        //}



        #endregion

        #region methods

        //public static void CheckInstall () {
        //    if (PathSelected) {
        //        PatchClient.InstallPath = GamePaths;
        //        PatchClient.UpdateCurrentInstallations();
        //        PatchClient.RequestVerifyVersions();
        //        PatchClient.VersionVerificationDone += PatchClient_VersionVerificationDone;
                
        //        //Get versions installed
        //        //Send for verification
        //        //Handle answer (missing files or verified)
        //        //If not verified, request files from specific version
        //        //Request download of missing files from previous request

        //    }
        //}

        private static void PatchClient_VersionVerificationDone () {
            //foreach (InstallationDataModel model in PatchClient.InstalledVersions) {
            //    if (!model.Verified) {

            //    }
            //}
        }

        public static void InstallVersion(string version) {
            PatchClient.RequestVersionMissingFiles(version);
            PatchClient.MissingFileListReceived += PatchClient_MissingFileListReceived;
        }

        private static void PatchClient_MissingFileListReceived (string versionName) {
            PatchClient.DownloadMissingFiles(versionName);
        }



        internal static void NewPath (string filename) {
            Settings.Default.GamePath = filename;
            Settings.Default.Save();
        }

        private static void PatchClient_VersionsFromServerReceived (VersionsFromServerRecievedEventArgs args) {
            throw new NotImplementedException();
        }





        /// <summary>
        /// Checks if two password SecureStrings are the same value
        /// </summary>
        /// <param name="_password"></param>
        /// <param name="_confirmPass"></param>
        /// <returns></returns>
        public static bool CheckPassUniformity (SecureString _password, SecureString _confirmPass) {
            string pass = ConvertToUnsecureString(_password);
            string confirmPass = ConvertToUnsecureString(_confirmPass);

            if (pass == confirmPass) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Converts a SecureString password to a normal readable string
        /// </summary>
        /// <param name="securePassword"></param>
        /// <returns></returns>
        public static string ConvertToUnsecureString (SecureString securePassword) {
            if (securePassword == null) {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/1570422/convert-string-to-securestring/43084626
        /// </summary>
        /// <param name="originalString"></param>
        /// <returns></returns>
        public static SecureString ConvertToSecureString (string originalString) {
            if (originalString == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in originalString)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
        #endregion
    }
}
