using System;
using StackExchange.Redis;

namespace Farseer.Net.Cache.Redis
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
        /// 根据指定的key从缓存中获取value
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T类型实例</returns>
        T Get<T>(string key);

        /// <summary>
        /// 将key及其value保存到缓存中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">data</param>
        /// <param name="cacheTime">缓存时间</param>
        bool Set<T>(string key, T data, int cacheTime);

        /// <summary>
        /// 根据key判断value是否缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否缓存</returns>
        bool Exists(string key);

        /// <summary>
        /// 根据指定的key移除value
        /// </summary>
        /// <param name="key">key</param>
        bool Remove(string key);

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
