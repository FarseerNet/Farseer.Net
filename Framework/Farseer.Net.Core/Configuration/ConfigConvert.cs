using System.Collections.Generic;
using Newtonsoft.Json;

namespace FS.Core.Configuration
{
    public class ConfigConvert
    {
        /// <summary>
        /// 将配置字符串换成配置类
        /// </summary>
        /// <param name="configValue">配置字符串：a=b,c=d,e=f</param>
        public static TConfig ToEntity<TConfig>(string configValue)
        {
            var dicConfig = new Dictionary<string, string>();
            foreach (var items in configValue.Split(','))
            {
                var kv = items.Split('=');
                if (kv.Length != 2) continue;
                dicConfig[kv[0]] = kv[1];
            }
            var dbConfig = Jsons.ToObject<TConfig>(JsonConvert.SerializeObject(dicConfig));
            return dbConfig;
        }
    }
}