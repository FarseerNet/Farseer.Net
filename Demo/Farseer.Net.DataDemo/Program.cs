using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS;
using FS.Utils.Common;

namespace Farseer.Net.DataDemo;

internal class Program
{
    private static async Task Main(string[] args)
    {
        FarseerApplication.Run<StartupModule>().Initialize();
        return;
        // 添加新用户（非事务）
        Console.WriteLine($"测试非事务：");
        await AddUser();

        Console.WriteLine($"测试事务：");
        // 添加新用户（使用事务的方式）
        using (var db = new MysqlContext())
        {
            db.AddCallback(() =>
            {
                Console.WriteLine(DateTime.Now);
            });
            await db.User.AddUpAsync(o => o.Age, 1);
            await AddUser(); // 由于属于new MysqlContext()内，此处则自动使用事务
            db.SaveChanges();
        }

        Console.WriteLine($"当前用户数：{await MysqlContext.Data.User.CountAsync()}");
    }

    private static Task AddUser()
    {
        return MysqlContext.Data.User.InsertAsync(new UserPO
        {
            Name = $"farseer-{Rand.GetRandom(1000, 9999)}",
            Age  = Rand.GetRandom(1, 100),
            Fullname = new FullNameVO
            {
                FirstName = "Farseer",
                LastName  = "Net"
            },
            Specialty = new List<string>() { "Farseer.Net.Data", "Farseer.Net.ElasticSearch", "Farseer.Net.EventBus", "Farseer.Net.Fss", "Farseer.Net.LinkTrack", "Farseer.Net.MQ.Queue", "Farseer.Net.MQ.Rabbit", "Farseer.Net.MQ.RedisStream", },
            Attribute = new Dictionary<string, string>()
            {
                { "Farseer.Net.Data.Ver", "2.6.1" },
                { "Farseer.Net.ElasticSearch.Ver", "2.6.1" }
            },
            Gender = GenderType.Man
        });
    }
}