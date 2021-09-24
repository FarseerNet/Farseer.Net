using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS;

namespace Farseer.Net.Demo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            var lst = new List<int>
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10
            };

            var dic = lst.ToDictionary(keySelector: o => o, elementSelector: ThrowAsync);
            await Task.WhenAll(tasks: dic.Select(selector: o => o.Value));
            foreach (var kv in dic) Console.WriteLine(value: kv.Value.Result);
            return;

            Thread.Sleep(millisecondsTimeout: -1);
        }

        public static async Task<int> ThrowAsync(int index)
        {
            await Task.Delay(millisecondsDelay: 5000);
            return index;
        }
    }
}