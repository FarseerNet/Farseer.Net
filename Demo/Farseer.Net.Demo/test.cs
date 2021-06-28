using System;
using System.Threading.Tasks;

namespace Farseer.Net.Demo
{
    class Program
    {
        static async Task<string> DoTaskAsync(string name, int timeout)
        {
            var start = DateTime.Now;
            Console.WriteLine("Enter {0}, {1}", name, timeout);
            await Task.Delay(timeout);
            Console.WriteLine("Exit {0}, {1}", name, (DateTime.Now - start).TotalMilliseconds);
            return name;
        }

        static async Task DoWork1()
        {
            var t1 = DoTaskAsync("t1.1", 3000);
            var t2 = DoTaskAsync("t1.2", 2000);
            var t3 = DoTaskAsync("t1.3", 1000);

            await t1;
            await t2;
            await t3;

            Console.WriteLine("DoWork1 results: {0}", String.Join(", ", await t1,await t2, await t3));
        }

        static async Task DoWork2()
        {
            var t1 = DoTaskAsync("t2.1", 3000);
            var t2 = DoTaskAsync("t2.2", 2000);
            var t3 = DoTaskAsync("t2.3", 1000);

            await Task.WhenAll(t1, t2, t3);

            Console.WriteLine("DoWork1 results: {0}", String.Join(", ", await t1, await t2, await t3));
        }


        static async Task Main(string[] args)
        {
            Task.WhenAll(DoWork1(), DoWork2()).Wait();
        }
    }
}