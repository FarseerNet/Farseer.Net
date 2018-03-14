// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 16:15
// ********************************************

using System.Collections.Generic;
using FS.Configuration;

namespace FS.Cache.Redis.Configuration
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisConfig : IFarseerConfig
    {
        /// <summary>
        /// Redis配置集合
        /// </summary>
        public List<RedisItemConfig> Items = new List<RedisItemConfig>();
    }
}