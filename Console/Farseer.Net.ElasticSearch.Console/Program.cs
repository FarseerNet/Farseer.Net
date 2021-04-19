
using System;
using FS;
using FS.DI;

namespace Farseer.Net.ElasticSearch.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            TestContext.Data.User.Insert(new UserPo
            {
                UserName = DateTime.Now.ToString()
            });
        }
    }
}