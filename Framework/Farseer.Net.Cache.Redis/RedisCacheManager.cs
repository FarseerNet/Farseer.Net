using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;
using FS.Cache.Redis.Configuration;

namespace FS.Cache.Redis
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
        ///     根据pattern移除项
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            foreach (var ep in _connectionWrapper.GetEndpoints())
            {
                var server = _connectionWrapper.Server(ep);
                var keys   = server.Keys(pattern: "*" + pattern + "*");
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
                var keys   = server.Keys();
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


        /// <summary>
        ///     事务，批量写入HASH
        /// </summary>
        public void HashSetTransaction<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> funcDataKey, Func<TEntity, string> funcData = null)
        {
            if (lst == null || lst.Count == 0) return;
            if (funcData == null) funcData = po => JsonConvert.SerializeObject(po);

            var transaction = Db.CreateTransaction();
            foreach (var po in lst)
            {
                var dataKey = funcDataKey(po).ToString();
                var data    = funcData(po);
                transaction.HashSetAsync(key, dataKey, data);
            }

            transaction.Execute();
        }
    }
}