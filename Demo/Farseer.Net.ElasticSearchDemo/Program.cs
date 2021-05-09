using System;
using FS;

namespace Farseer.Net.ElasticSearchDemo
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