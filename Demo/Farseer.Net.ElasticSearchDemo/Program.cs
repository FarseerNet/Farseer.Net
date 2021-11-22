using System;
using System.Linq.Expressions;
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
                Id       = Guid.NewGuid().ToString(),
                UserName = "steden",
                Age      = DateTime.Now.Second
            });

            var lst1 = TestContext.Data.User.Where(o => o.UserName != "steden").ToList();
            var lst2 = TestContext.Data.User.Where(o => o.Age      == 30).Where(o=>o.UserName == "aaaa").ToList();
            var lst3 = TestContext.Data.User.Where(o => o.UserName == "steden" && o.Age == 18).ToList();
            var lst4 = TestContext.Data.User.Where(o => o.UserName == "steden" || o.Age >= 10).ToList();
        }
    }
}