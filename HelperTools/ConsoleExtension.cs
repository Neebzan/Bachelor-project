using System;
using System.Collections.Generic;
using System.Text;

namespace HelperTools {
   public static class ConsoleExtension {
        public static void WriteLineTimestamp (string msg) {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "  -  " + msg);
        }
    }
}
