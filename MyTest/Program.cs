﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var demo = new MyTest.Tasks.TaskDemo();
            //demo.Test2();

            var demo = new MyTest.Collection.CollectionDemo();
            demo.Test1();

            Console.ReadLine();
        }
    }
}
