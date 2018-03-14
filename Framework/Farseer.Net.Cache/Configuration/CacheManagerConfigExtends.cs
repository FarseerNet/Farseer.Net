// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-29 13:58
// ********************************************

using FS.Cache.Configuration;
using FS.Configuration;

// ReSharper disable once CheckNamespace
namespace Farseer.Net.Configuration
{
    /// <summary>
    /// CacheManager配置扩展
    /// </summary>
    public static class CacheManagerConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static CacheManagerConfig CacheManagerConfig(this IConfigResolver resolver) => resolver.Get<CacheManagerConfig>() ?? new CacheManagerConfig();
    }
}