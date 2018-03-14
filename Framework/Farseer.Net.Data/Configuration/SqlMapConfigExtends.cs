// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-29 13:58
// ********************************************

using FS.Data.Configuration;

// ReSharper disable once CheckNamespace
namespace FS.Configuration
{
    /// <summary>
    /// Db配置扩展
    /// </summary>
    public static class SqlMapConfigExtends
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        public static SqlMapConfig SqlMapConfig(this IConfigResolver resolver) => resolver.Get<SqlMapConfig>();
    }
}