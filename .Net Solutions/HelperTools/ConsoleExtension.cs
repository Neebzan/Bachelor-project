using System;
using System.Collections.Generic;
using System.Text;

namespace HelperTools {
   public static class ConsoleExtension {
        public static string AddTimestamp (string msg) {
            return DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "  -  " + msg;
        }
    }
}
