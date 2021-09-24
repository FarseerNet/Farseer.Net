using System;
using FS;

namespace Farseer.Net.ElasticSearchDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FarseerApplication.Run<StartupModule>().Initialize();

            TestContext.Data.User.Insert(model: new UserPo
            {
                UserName = DateTime.Now.ToString()
            });
        }
    }
}