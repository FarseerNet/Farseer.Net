using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Cache.Redis.Configuration;
using FS.DI;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis缓存管理器
    /// </summary>
    public class RedisCacheManager : IRedisCacheManager
    {
        private readonly RedisItemConfig _config;

        /// <summary>
        ///     Redis连接包装器
        /// </summary>
        private readonly IRedisConnectionWrapper _connectionWrapper;

        /// <summary>
        ///     数据库
        /// </summary>
        private IDatabase _db;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="config"> 配置 </param>
        public RedisCacheManager(RedisItemConfig config)
        {
            Check.NotNull(value: config.Server, parameterName: "Redis连接字符串为空");
            _config            = config;
            _connectionWrapper = new RedisConnectionWrapper(config);
        }

        /// <summary>
        ///     数据库
        /// </summary>
        public IDatabase Db
        {
            get { return _db ??= new LinkTrackWarp(db: _connectionWrapper.Database()); }
        }

        /// <summary>
        ///     支持缓存不存在，则写入
        /// </summary>
        public ICacheManager CacheManager => IocManager.GetService<ICacheManager>(name: $"GetCacheInMemory_{_config.Name}");

        /// <summary>
        /// 事务锁
        /// </summary>
        /// <param name="key">KEY</param>
        /// <param name="lockTime">锁的时长</param>
        public RedisLock GetLocker(string key, TimeSpan lockTime) => new(Db, key, lockTime);

        /// <summary>
        ///     Redis服务端
        /// </summary>
        public string Server => _config.Server;

        /// <summary>
        ///     清楚所有缓存数据
        /// </summary>
        public virtual void Clear()
        {
            foreach (var ep in _connectionWrapper.GetEndpoints())
            {
                var server = _connectionWrapper.Server(endPoint: ep);
                var keys   = server.Keys();
                foreach (var key in keys) Db.KeyDelete(key: key);
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
        public void HashSetTransaction<TEntity, TEntityId>(string key, PooledList<TEntity> lst, Func<TEntity, TEntityId> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null)
        {
            if (lst == null || lst.Count == 0) return;
            if (funcData == null) funcData = po => JsonConvert.SerializeObject(value: po);

            var       transaction = Db.CreateTransaction();
            using var tasks       = new PooledList<Task>();
            foreach (var po in lst)
            {
                var dataKey = funcDataKey(arg: po).ToString();
                var data    = funcData(arg: po);
                tasks.Add(item: transaction.HashSetAsync(key: key, hashField: dataKey, value: data));
            }

            // 设置过期时间
            if (expiry != null) transaction.KeyExpireAsync(key: key, expiry: expiry.GetValueOrDefault());
            transaction.Execute();
            Task.WaitAll(tasks: tasks.ToArray());
        }

        /// <summary>
        ///     事务，批量写入HASH
        /// </summary>
        public Task HashSetTransactionAsync<TEntity, TEntityId>(string key, PooledList<TEntity> lst, Func<TEntity, TEntityId> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null)
        {
            if (lst == null || lst.Count == 0) return Task.FromResult(result: 0);
            funcData ??= po => JsonConvert.SerializeObject(value: po);

            var transaction = Db.CreateTransaction();
            foreach (var po in lst)
            {
                var dataKey = funcDataKey(arg: po).ToString();
                var data    = funcData(arg: po);
                transaction.HashSetAsync(key: key, hashField: dataKey, value: data);
            }

            // 设置过期时间
            if (expiry != null) transaction.KeyExpireAsync(key: key, expiry: expiry.GetValueOrDefault());
            return transaction.ExecuteAsync();
        }
    }
}