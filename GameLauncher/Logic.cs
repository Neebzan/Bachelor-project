using GameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher {
    public static class Logic {
        #region Properties

        private static bool pathSelected;

        public static bool PathSelected {
            get {
                //Should run checks on wether or not this is a valid path
                return !String.IsNullOrEmpty(GamePath);
            }
            set { pathSelected = value; }
        }

        private static bool gameInstalled;

        public static bool GameInstalled {
            get {
                return PathSelected;
            }
            set { gameInstalled = value;}
        }

        public static string GamePath {
            get {
                return Settings.Default.GamePath;
            }
            set {
                Settings.Default.GamePath = value;
            }
        }

        #endregion

        #region methods
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
