using System;

namespace PatchManager
{
    class Program
    {
        static void Main(string[] args)
        {
            PatchServer t = new PatchServer();
            t.Start();
            t.Stop();
        }
    }
}
