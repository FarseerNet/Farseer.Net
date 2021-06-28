using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS;
using FS.DI;

namespace Farseer.Net.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            var lst = new List<int>()
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10
            };

            var dic = lst.ToDictionary(o => o, ThrowAsync);
            await Task.WhenAll(dic.Select(o => o.Value));
            foreach (var kv in dic)
            {
                Console.WriteLine(kv.Value.Result);
            }
            return;
            
            Thread.Sleep(-1);
        }

        public static async Task<int> ThrowAsync(int index)
        {
            await Task.Delay(5000);
            return index;
        }
    }
}