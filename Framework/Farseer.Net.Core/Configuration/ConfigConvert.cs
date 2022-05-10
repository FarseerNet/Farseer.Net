using System.Collections.Generic;
using FS.Cache;

namespace FS.Core.Configuration
{
    public class ConfigConvert
    {
        /// <summary>
        /// 将配置字符串换成配置类 139ms
        /// </summary>
        /// <param name="configValue">配置字符串：a=b,c=d,e=f</param>
        public static TConfig ToEntity<TConfig>(string configValue) where TConfig : new()
        {
            var dicConfig = new Dictionary<string, string>();
            foreach (var items in configValue.Split(','))
            {
                var kv = items.Split('=');
                if (kv.Length != 2) continue;
                dicConfig[kv[0]] = kv[1];
            }

            var config = new TConfig();
            foreach (var propertyInfo in typeof(TConfig).GetProperties())
            {
                if (dicConfig.ContainsKey(propertyInfo.Name))
                {
                    if (propertyInfo.PropertyType == typeof(bool))
                    {
                        bool.TryParse(dicConfig[propertyInfo.Name], out var result);
                        PropertySetCacheManger.Cache(propertyInfo, config, result);
                    }
                    else if (propertyInfo.PropertyType == typeof(int))
                    {
                        int.TryParse(dicConfig[propertyInfo.Name], out var result);
                        PropertySetCacheManger.Cache(propertyInfo, config, result);
                    }
                    else if (propertyInfo.PropertyType == typeof(long))
                    {
                        long.TryParse(dicConfig[propertyInfo.Name], out var result);
                        PropertySetCacheManger.Cache(propertyInfo, config, result);
                    }
                    else PropertySetCacheManger.Cache(propertyInfo, config, dicConfig[propertyInfo.Name]);
                }
            }

            return config;
        }
    }
}