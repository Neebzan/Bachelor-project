using ChecksumHandlerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            ChecksumTool.GetFilesDictionary();
            Dictionary<string, string> testDic = new Dictionary<string, string>();
            //ChecksumTool.GetFilesDictionary(out testDic, @"C:\Users\Henrik\Desktop\backups");
            //ChecksumTool.GetFilesDictionary(out testDic, @"TestFolder\TestFolder2");
            ChecksumTool.GetFilesDictionary(out testDic);


            Console.WriteLine("Yeet");
            ChecksumTool.HelloWorld();
            //Console.ReadKey();
        }
    }
}
