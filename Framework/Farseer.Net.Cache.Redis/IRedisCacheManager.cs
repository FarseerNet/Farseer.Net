using System;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    /// <summary>
    /// 缓存管理接口
    /// </summary>
    public interface IRedisCacheManager : IDisposable
    {
        /// <summary>
        ///     数据库
        /// </summary>
        IDatabase Db { get; }

        /// <summary>
        /// 根据patten移除项
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        void Clear();
    }
}
