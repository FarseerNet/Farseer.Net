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
                Age      = DateTime.Now.Second,
                Desc     = $"大家好，我是steden,我今年{DateTime.Now.Second}岁"
            });

            TestContext.Data.User.Where(q => q.Term(t => t.Age, 33)).Where(o => o.UserName.Contains("ste")).ToList();
            TestContext.Data.User.Where(o => o.Desc.Contains("我今年")).Asc(o => o.Age).ToList();
            TestContext.Data.User.Where(o => o.Desc.StartsWith("大家好")).Desc(o => o.Age).ToList();
            TestContext.Data.User.Where(o => o.UserName.EndsWith("en")).ToList();
            TestContext.Data.User.Where(o => o.UserName != "aaa").ToList();
            TestContext.Data.User.Where(o => o.Age      == 30).Where(o => o.UserName == "steden").ToList();
            TestContext.Data.User.Where(o => o.UserName == "steden" && o.Age == 18).ToList();
            TestContext.Data.User.Where(o => o.UserName == "steden" || o.Age >= 10).ToList();
        }
    }
}