// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 16:16
// ********************************************
namespace Farseer.Net.Cache.Configuration
{
    /// <summary>
    /// CacheManager配置
    /// </summary>
    public class CacheManagerItemConfig
    {
        /// <summary> 缓存名称 </summary> 
        public string Name = ".";

        /// <summary> 缓存模式 </summary> 
        public EumCacheModel CacheModel = EumCacheModel.Runtime;

        /// <summary> Redis配置名称 </summary>
        public string RedisConfigName = "";
    }
}