using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GameLauncher {
    public static class Utility {

        /// <summary>
        /// Converts a SecureString to a normal readable string
        /// </summary>
        /// <param name="secureString"></param>
        /// <returns></returns>
        public static string ConvertToUnsecureString (SecureString secureString) {
            if (secureString == null)
                return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }

            finally {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string HashedString (string _input) {
            using (HMACSHA512 t = new HMACSHA512(Encoding.UTF8.GetBytes(_input))) {
                byte [ ] hash;
                hash = t.ComputeHash(Encoding.UTF8.GetBytes(_input));
                _input = BitConverter.ToString(hash).Replace("-", "");
            }

            return _input;
        }
    }
}
