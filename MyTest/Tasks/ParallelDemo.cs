using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTest.Tasks
{
    /// <summary>
    /// 随着多核时代的到来，并行开发越来越展示出它的强大威力！使用并行程序，充分的利用系统资源，提高程序的性能。
    /// 在.net 4.0中，微软给我们提供了一个新的命名空间：System.Threading.Tasks。
    /// 最基础，最简单的——认识和使用Parallel
    /// </summary>
    public class ParallelDemo
    {
        private Stopwatch stopWatch = new Stopwatch();

        public void Run1()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Task 1 is cost 2 sec");
        }
        public void Run2()
        {
            Thread.Sleep(3000);
            Console.WriteLine("Task 2 is cost 3 sec");
        }

        //总执行时间是3秒，可以看出方法是并行执行的，执行效率提高了很多
        public void ParallelInvokeMethod()
        {
            stopWatch.Start();
            Parallel.Invoke(Run1, Run2);
            stopWatch.Stop();
            Console.WriteLine("Parallel run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Restart();
            Run1();
            Run2();
            stopWatch.Stop();
            Console.WriteLine("Normal run " + stopWatch.ElapsedMilliseconds + " ms.");
        }

        //同样为了消耗CPU计算，可以看出Parallel.For用了很少的时间就完成了执行
        public void ParallelForMethod()
        {
            stopWatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 60000; j++)
                {
                    int sum = 0;
                    sum += i;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("NormalFor run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            Parallel.For(0, 10000, item =>
            {
                for (int j = 0; j < 60000; j++)
                {
                    int sum = 0;
                    sum += item;
                }
            });
            stopWatch.Stop();
            Console.WriteLine("ParallelFor run " + stopWatch.ElapsedMilliseconds + " ms.");
        }

        //Parallel.For由于是并行运行的，所以会同时访问全局变量num,为了得到正确的结果，要使用lock,此时来看看运行结果
        public void ParallelForMethod1()
        {
            var obj = new Object();
            long num = 0;

            stopWatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 60000; j++)
                {
                    num++;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("NormalFor run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            Parallel.For(0, 10000, item =>
            {
                for (int j = 0; j < 60000; j++)
                {
                    lock (obj)
                    {
                        num++;
                    }
                }
            });
            stopWatch.Stop();
            Console.WriteLine("ParallelFor run " + stopWatch.ElapsedMilliseconds + " ms.");
        }
        //这个方法跟Foreach方法很相似，可以看出使用了Parallel，就不是按顺序显示了
        public void ParallelForEachMethod()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
            }
            Parallel.ForEach(list, item =>
            {
                Console.WriteLine(item);
            });
        }
        /// <summary>
        /// Break: 当然这个是通知并行计算尽快的退出循环，比如并行计算正在迭代100，那么break后程序还会迭代所有小于100的。
        /// Stop：这个就不一样了，比如正在迭代100突然遇到stop，那它啥也不管了，直接退出。
        /// 这里使用的是Stop，当数量达到300个时，会立刻停止；可以看到结果"Bag count is 300"，如果用break,可能结果是300多个或者300个
        /// </summary>
        public void ParallelBreak()
        {
            ConcurrentBag<int> list = new ConcurrentBag<int>();
            stopWatch.Start();
            Parallel.For(0, 1000, (i, state) =>
            {
                if (list.Count == 300)
                {
                    state.Break();
                    return;
                }
                list.Add(i);
            });
            stopWatch.Stop();
            Console.WriteLine("Bag count is " + list.Count + ", " + stopWatch.ElapsedMilliseconds);
        }
        //我们会发现结果有可能不是10000，是由于线程不安全导致，要使用下面方法的ConcurrentBag
        public void ListWithParallel()
        {
            List<int> list = new List<int>();
            Parallel.For(0, 10000, item =>
            {
                list.Add(item);
            });
            Console.WriteLine("List's count is {0}", list.Count());
        }
        //关于线程安全的集合还有很多，和我们平时用的集合都差不多，比如类似Dictionary的ConcurrentDictionary，还有ConcurrentStack，ConcurrentQueue等
        public void ConcurrentBagWithPallel()
        {
            ConcurrentBag<int> list = new ConcurrentBag<int>();
            Parallel.For(0, 10000, item =>
            {
                list.Add(item);
            });
            Console.WriteLine("ConcurrentBag's count is {0}", list.Count());
        }
        #region Linq的并行版本
        public void TestPLinq()
        {
            Stopwatch sw = new Stopwatch();
            List<Custom> customs = new List<Custom>();
            for (int i = 0; i < 2000000; i++)
            {
                customs.Add(new Custom() { Name = "Jack", Age = 21, Address = "NewYork" });
                customs.Add(new Custom() { Name = "Jime", Age = 26, Address = "China" });
                customs.Add(new Custom() { Name = "Tina", Age = 29, Address = "ShangHai" });
                customs.Add(new Custom() { Name = "Luo", Age = 30, Address = "Beijing" });
                customs.Add(new Custom() { Name = "Wang", Age = 60, Address = "Guangdong" });
                customs.Add(new Custom() { Name = "Feng", Age = 25, Address = "YunNan" });
            }

            sw.Start();
            var result = customs.Where<Custom>(c => c.Age > 26).ToList();
            sw.Stop();
            Console.WriteLine("Linq time is {0}.", sw.ElapsedMilliseconds);

            sw.Restart();
            sw.Start();
            var result2 = customs.AsParallel().Where<Custom>(c => c.Age > 26).ToList();
            sw.Stop();
            Console.WriteLine("Parallel Linq time is {0}.", sw.ElapsedMilliseconds);
        }
        //分组统计ToLookup方法
        public void OrderByTest()
        {
            Stopwatch stopWatch = new Stopwatch();
            List<Custom> customs = new List<Custom>();
            for (int i = 0; i < 1000000; i++)
            {
                customs.Add(new Custom() { Name = "Jack", Age = 21, Address = "NewYork" });
                customs.Add(new Custom() { Name = "Jime", Age = 26, Address = "China" });
                customs.Add(new Custom() { Name = "Tina", Age = 29, Address = "ShangHai" });
                customs.Add(new Custom() { Name = "Luo", Age = 30, Address = "Beijing" });
                customs.Add(new Custom() { Name = "Wang", Age = 60, Address = "Guangdong" });
                customs.Add(new Custom() { Name = "Feng", Age = 25, Address = "YunNan" });
            }

            stopWatch.Restart();
            var groupByAge = customs.GroupBy(item => item.Age).ToList();
            foreach (var item in groupByAge)
            {
                Console.WriteLine("Age={0},count = {1}", item.Key, item.Count());
            }
            stopWatch.Stop();

            Console.WriteLine("Linq group by time is: " + stopWatch.ElapsedMilliseconds);


            stopWatch.Restart();
            var lookupList = customs.ToLookup(i => i.Age);
            foreach (var item in lookupList)
            {
                Console.WriteLine("LookUP:Age={0},count = {1}", item.Key, item.Count());
            }
            stopWatch.Stop();
            Console.WriteLine("LookUp group by time is: " + stopWatch.ElapsedMilliseconds);
        }
        private class Custom
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Address { get; set; }
        }
        #endregion
    }
}
