using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS;
using FS.Core.Http;
using FS.Extends;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Farseer.Net.Demo
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();
            var result = await HttpGet.GetAsync("http://cdn.es3.com/?www.baidu.com");
            Console.WriteLine(result);
        }

        public static async Task<int> ThrowAsync(int index)
        {
            await Task.Delay(millisecondsDelay: 5000);
            return index;
        }
    }
}