using System.Collections.Generic;
using System.Threading;
using Collections.Pooled;
using FS;
using FS.Cache;
using FS.Core.Abstract.Cache;
using FS.Core.Net;
using FS.DI;
using NUnit.Framework;

namespace Farseer.Net.Cache.Test;

public class CacheManager : BaseTests
{
    [Test]
    public void LocalCache()
    {
        // 发送数据
        var cacheManager = IocManager.GetService<ICacheManager>();
        var cacheKey     = new CacheKey<ApiResponseJson, int>("apiKey", o => o.StatusCode, EumCacheStoreType.Memory);

        // 获取列表，此时列表数量=0
        var lst = cacheManager.GetList(cacheKey, () => new PooledList<ApiResponseJson>());
        Assert.Zero(lst.Count);

        // 添加item
        cacheManager.SaveItem(cacheKey, new ApiResponseJson() { StatusCode = 1 });
        var count = cacheManager.GetCount(cacheKey);
        Assert.AreEqual(count, 1);

        // 获取刚添加的item
        var apiResponseJson = cacheManager.GetItem(cacheKey, 1);
        Assert.NotNull(apiResponseJson);
        Assert.AreEqual(apiResponseJson.StatusCode, 1);

        // 保存列表
        cacheManager.SaveList(cacheKey, new List<ApiResponseJson>()
        {
            new() { StatusCode = 2 },
            new() { StatusCode = 3 }
        });

        // 获取数量，此时数量应为2
        count = cacheManager.GetCount(cacheKey);
        Assert.AreEqual(count, 2);
    }
}