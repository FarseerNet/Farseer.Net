using System;
using System.Linq.Expressions;
using FS;
using FS.Extends;

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
                Desc     = $"大家好，我是steden,我今年{DateTime.Now.Second}岁",
                CreateAt = DateTime.Now.ToTimestamps()
            });

            var time = "30";
            // 判断时间（并带有复杂的本地函数方法）
            TestContext.Data.User.Where(o => o.CreateAt >= DateTime.Now.AddMinutes(-time.ConvertType(0)).ToTimestamps()).ToList();
            // NETS原生的条件 + 自解析的条件
            TestContext.Data.User
                       .Where(q => q.Term(t => t.Age, 33))
                       .Where(o => o.UserName.Contains("ste")).ToList();
            // 模糊搜索 + 正序排序
            TestContext.Data.User.Where(o => o.Desc.Contains("我今年")).Asc(o => o.Age).ToList();
            // 前缀搜索 + 倒序排序
            TestContext.Data.User.Where(o => o.Desc.StartsWith("大家好")).Desc(o => o.Age).ToList();
            // 后缀搜索（只支持Keyword类型）
            TestContext.Data.User.Where(o => o.UserName.EndsWith("en")).ToList();
            // 不等于某个值
            TestContext.Data.User.Where(o => o.UserName != "aaa").ToList();
            // and 运算，如果两个Where方法调用，也相当于使用and
            TestContext.Data.User.Where(o => o.UserName == "steden" && o.Age == 18).ToList();
            // or 运行
            TestContext.Data.User.Where(o => o.UserName == "steden" || o.Age >= 10).ToList();
        }
    }
}