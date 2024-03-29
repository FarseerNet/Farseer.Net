using System.Collections.Generic;
using Collections.Pooled;
using FS.Core.Configuration;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Data.Configuration
{
    /// <summary>
    ///     读取Data配置
    /// </summary>
    public class DataConfigRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static IEnumerable<DbItemConfig> Get()
        {
            using var configs = IocManager.GetService<IConfigurationRoot>().GetSection(key: "Database").GetChildren().ToPooledList();
            foreach (var configurationSection in configs)
            {
                var config = ConfigConvert.ToEntity<DbItemConfig>(configurationSection.Value);
                if (config == null) continue;

                if (config.Server.Contains(":"))
                {
                    var servers = config.Server.Split(':');
                    config.Server = servers[0];
                    config.Port   = servers[1];
                }
                if (config.ConnectTimeout == 0) config.ConnectTimeout = 600;
                if (config.CommandTimeout == 0) config.CommandTimeout = 300;
                config.Name = configurationSection.Key;
                yield return config;
            }
        }
    }

}