using System;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;
using Farseer.Net.Cache.Redis.Configuration;

namespace Farseer.Net.Cache.Redis
{
    /// <summary>
    ///     Redis缓存管理器
    /// </summary>
    public class RedisCacheManager : IRedisCacheManager
    {
        /// <summary>
        ///     Redis连接包装器
        /// </summary>
        private readonly IRedisConnectionWrapper _connectionWrapper;

        /// <summary>
        ///     数据库
        /// </summary>
        private IDatabase _db;

        /// <summary>
        ///     数据库
        /// </summary>
        public IDatabase Db => _db ?? (_db = _connectionWrapper.Database());

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="connectionWrapper">Redis连接</param>
        public RedisCacheManager(RedisItemConfig config, IRedisConnectionWrapper connectionWrapper)
        {
            Check.NotNull(config.Server, "Redis连接字符串为空");
            _connectionWrapper = connectionWrapper;
        }

        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="item">序列化对象</param>
        /// <returns>byte[]</returns>
        protected virtual byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="serializedObject">序列化对象</param>
        /// <returns>T类型实例</returns>
        protected virtual T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null) return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        ///     根据key从缓存中获取值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>T类型实例</returns>
        public virtual T Get<T>(string key)
        {
            var rValue = Db.StringGet(key);
            if (!rValue.HasValue) return default(T);
            var result = Deserialize<T>(rValue);

            return result;
        }

        /// <summary>
        ///     设置key及其关联的数据到缓存中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">data</param>
        /// <param name="cacheTime">缓存时间</param>
        public virtual bool Set<T>(string key, T data, int cacheTime)
        {
            if (data == null) return false;

            var entryBytes = Serialize(data);
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            return Db.StringSet(key, entryBytes, expiresIn);
        }

        /// <summary>
        ///     设置key及其关联的数据到缓存中
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">data</param>
        /// <param name="cacheTime">缓存时间</param>
        public virtual bool Set<T>(string key, T data, TimeSpan? cacheTime)
        {
            if (data == null) return false;

            var entryBytes = Serialize(data);
            Db.StringSet(key, entryBytes, cacheTime);

            return false;
        }

        /// <summary>
        ///     判断是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否存在</returns>
        public virtual bool Exists(string key) => Db.KeyExists(key);

        /// <summary>
        ///     根据指定的key从缓存中移除值
        /// </summary>
        /// <param name="key">key</param>
        public virtual bool Remove(string key) => Db.KeyDelete(key);

        /// <summary>
        ///     根据pattern移除项
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            foreach (var ep in _connectionWrapper.GetEndpoints())
            {
                var server = _connectionWrapper.Server(ep);
                var keys = server.Keys(pattern: "*" + pattern + "*");
                foreach (var key in keys) Db.KeyDelete(key);
            }
        }

        /// <summary>
        ///     清楚所有缓存数据
        /// </summary>
        public virtual void Clear()
        {
            foreach (var ep in _connectionWrapper.GetEndpoints())
            {
                var server = _connectionWrapper.Server(ep);
                var keys = server.Keys();
                foreach (var key in keys) Db.KeyDelete(key);
            }
        }

        /// <summary>
        ///     清理资源
        /// </summary>
        public virtual void Dispose()
        {
            //if (_connectionWrapper != null)
            //    _connectionWrapper.Dispose();
        }
    }
}