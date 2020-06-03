using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ServerManager {
    class Program {
        public static bool Running { get; private set; } = true;

        static void Main (string [ ] args) {
            Console.WriteLine("With serverlist functionality!");
            ServerManager.Init();


            while (Running) {
                Thread.Sleep(100);
            }
        }
    }
}
