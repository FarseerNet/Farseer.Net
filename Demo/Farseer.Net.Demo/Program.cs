using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS;
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
            var message = "{\"ErrorCount\":1,\"aaa\":{\"bbbb\":1,\"cccc\":{}}}";
            var json    = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
            if (json.ContainsKey("ErrorCount"))
            {
                json["ErrorCount"] = json["ErrorCount"].ConvertType(0) + 1;
            }

            Console.WriteLine(JsonConvert.SerializeObject(json));
            Thread.Sleep(millisecondsTimeout: -1);
        }

        public static async Task<int> ThrowAsync(int index)
        {
            await Task.Delay(millisecondsDelay: 5000);
            return index;
        }
    }
}