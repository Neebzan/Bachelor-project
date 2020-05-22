using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerManager {
    class Program {
        public static bool Running { get; private set; }

        static void Main (string [ ] args) {
            ServerManager.Init();


            while (Running) {
                Console.ReadKey();
            }
        }
    }
}
