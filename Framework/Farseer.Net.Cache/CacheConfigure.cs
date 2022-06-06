using System;
using System.Collections.Generic;
using FS.Core.Abstract.Cache;

namespace FS.Cache;

public class CacheConfigure
{
    internal static Dictionary<string, CacheKey2> Configure = new();

    public static CacheKey2 Get(string key)
    {
        if (!Configure.TryGetValue(key, out var cacheKey)) throw new Exception($"使用Cache缓存，需提前初始化CacheConfigure：ICacheServices.SetProfiles");
        return cacheKey;
    }
}