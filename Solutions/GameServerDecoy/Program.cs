﻿using Newtonsoft.Json;
using ServerManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameServerDecoy {
    class Program {
        static void Main (string [ ] args) {
            while (true) {
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                    Task.Run(() => EmulatedUserConnection.Init());                
            }
        }
    }
}
