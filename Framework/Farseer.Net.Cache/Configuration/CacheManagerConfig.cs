// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 16:15
// ********************************************

using System.Collections.Generic;
using FS.Configuration;

namespace FS.Cache.Configuration
{
    /// <summary>
    /// CacheManager配置
    /// </summary>
    public class CacheManagerConfig : IFarseerConfig
    {
        /// <summary>
        /// Redis配置集合
        /// </summary>
        public List<CacheManagerItemConfig> Items = new List<CacheManagerItemConfig>();
    }
}