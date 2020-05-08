using System;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Start(10, 13000);

            Console.ReadKey();
        }
    }
}
