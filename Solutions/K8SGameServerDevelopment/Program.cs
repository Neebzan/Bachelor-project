using System;
using System.Threading;

namespace K8SGameServerDevelopment {
    class Program {
        static void Main (string [ ] args) {
            Console.WriteLine("Server started");
            Console.WriteLine("Sleeping for 10 seconds");
            Thread.Sleep(10000);
            Console.WriteLine("kill");
        }
    }
}
