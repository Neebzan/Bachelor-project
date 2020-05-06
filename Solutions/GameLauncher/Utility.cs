using DatabaseREST.Models;
using GameLauncher.Properties;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

        public static async Task<bool> RefreshToken () {
            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("/token", Method.POST);

            request.AddHeader("token", Settings.Default.RefreshToken);

            var cancellationTokenSource = new CancellationTokenSource();

            try {
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var tokens = JsonConvert.DeserializeObject<TokenModel>(response.Content);

                    Settings.Default.AccessToken = tokens.AccessToken;
                    Settings.Default.Save();

                    return true;
                }
                else {
                    return false;
                }
            }

            // Exception
            catch (Exception e) {
                return false;
            }

        }
    }
}
