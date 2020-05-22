using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerManager {
    class Program {
        public static bool Running { get; private set; } = true;

        static void Main (string [ ] args) {
            ServerManager.Init();


            while (Running) {
                var key =  Console.ReadKey();
                if (key.Key == ConsoleKey.Escape) {
                    Running = false;
                }
            }
        }
    }
}
