using System;
using System.Threading;

namespace PatchManager
{
    class Program
    {
        static void Main(string[] args)
        {
            PatchServer t = new PatchServer();
            t.Start();
            //t.Stop();
            while(true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
