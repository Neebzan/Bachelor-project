using Newtonsoft.Json;
using ServerManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace K8SGameServerDevelopment {
    class Program {
        public static bool Running { get; private set; } = true;

        static void Main (string [ ] args) {
            Console.WriteLine("Server started");

            Task.Run(() => Gameserver.Init());

            while (Running) {
                Thread.Sleep(100);
            }
        }
    }
}
