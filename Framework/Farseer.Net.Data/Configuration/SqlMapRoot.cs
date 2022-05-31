using System.Collections.Generic;
using Collections.Pooled;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Data.Configuration;

public class SqlMapRoot
{
    /// <summary>
    ///     读取配置
    /// </summary>
    public static IEnumerable<SqlMapItemConfig> Get()
    {
        using var configs = IocManager.GetService<IConfigurationRoot>().GetSection(key: "SqlMap").GetChildren().ToPooledList();
        foreach (var configurationSection in configs)
        {
            yield return new SqlMapItemConfig
            {
                Name = configurationSection.Key,
                Sql  = configurationSection.Value
            };
        }
    }
}