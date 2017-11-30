using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest.Log
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Default.Debug("Test");
            Console.ReadLine();
        }
    }
}
