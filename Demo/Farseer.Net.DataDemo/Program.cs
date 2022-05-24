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

        // 添加新用户
        await MysqlContext.Data.User.InsertAsync(new UserPO
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

        // 工作单元模式，非事务
        var lst = await MysqlContext.Data.User.ToListAsync();
        foreach (var taskGroupPO in lst)
        {
            Console.WriteLine(taskGroupPO.Name);
        }
    }
}