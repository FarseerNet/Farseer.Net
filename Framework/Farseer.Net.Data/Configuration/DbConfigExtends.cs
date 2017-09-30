// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-29 13:58
// ********************************************

using Farseer.Net.Data.Configuration;

// ReSharper disable once CheckNamespace
namespace Farseer.Net.Configuration
{
    /// <summary>
    /// Db配置扩展
    /// </summary>
    public static class DbConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static DbConfig DbConfig(this IConfigResolver resolver) => resolver.Get<DbConfig>();
    }
}