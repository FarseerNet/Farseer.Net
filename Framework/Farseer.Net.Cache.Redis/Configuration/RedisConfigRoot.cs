using Collections.Pooled;
using FS.Core.Configuration;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Cache.Redis.Configuration;

/// <summary>
///     读取Redis配置
/// </summary>
public class RedisConfigRoot
{
    /// <summary>
    ///     读取配置
    /// </summary>
    public static PooledList<RedisItemConfig> Get()
    {
        var lst = new PooledList<RedisItemConfig>();

        using var configs = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Redis").GetChildren().ToPooledList();
        foreach (var configurationSection in configs)
        {
            var config = ConfigConvert.ToEntity<RedisItemConfig>(configValue: configurationSection.Value);
            if (config == null) continue;
            config.Name = configurationSection.Key;
            lst.Add(config);
        }
        return lst;
    }
}