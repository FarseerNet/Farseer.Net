using System;
using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS;
using FS.Cache.Redis;
using FS.Cache.Redis.Configuration;
using FS.DI;
using NUnit.Framework;

namespace Farseer.Net.Cache.Redis.Test;

public class RedisCacheManager : BaseTests
{
    [Test]
    public void Db_NotNull()
    {
        var redisCacheManager = IocManager.GetService<IRedisCacheManager>();
        Assert.NotNull(redisCacheManager.Db);
    }

    [Test]
    public void Locker()
    {
        var redisCacheManager = IocManager.GetService<IRedisCacheManager>();
        var redisLock         = redisCacheManager.GetLocker(FarseerApplication.AppName, TimeSpan.FromSeconds(3));
        using (redisLock)
        {
            Assert.IsTrue(redisLock.TryLock());
            Assert.IsTrue(redisCacheManager.Db.KeyExists(FarseerApplication.AppName));
        }
        Assert.IsFalse(redisCacheManager.Db.KeyExists(FarseerApplication.AppName));
    }

    [Test]
    public void BatchOperate()
    {
        var redisCacheManager = IocManager.GetService<IRedisCacheManager>();
        var lst = new List<RedisItemConfig>
        {
            new() { Name = "1", Server = "2" },
            new() { Name = "2", Server = "3" },
            new() { Name = "3", Server = "4" },
        };
        var key = "test_BatchOperate";
        redisCacheManager.HashSetTransaction(key, lst, o => o.Name);
        Assert.IsTrue(redisCacheManager.Db.KeyExists(key));

        var redisItemConfigs = redisCacheManager.HashToList<RedisItemConfig>(key).ToPooledList();
        Assert.NotZero(redisItemConfigs.Count());
        for (int i = 0; i < redisItemConfigs.Count; i++)
        {
            var curItem = lst.FirstOrDefault(o => o.Name == redisItemConfigs[i].Name);
            Assert.AreEqual(redisItemConfigs[i].Name, curItem.Name);
            Assert.AreEqual(redisItemConfigs[i].Server, curItem.Server);
        }

        var redisItemConfig = redisCacheManager.HashToEntity<RedisItemConfig>(key, "3");
        Assert.NotNull(redisItemConfig);

        var toEntity = lst.FirstOrDefault(o => o.Name == redisItemConfig.Name);
        Assert.AreEqual(toEntity.Server, redisItemConfig.Server);
        redisCacheManager.Db.KeyDelete(key);
    }
}