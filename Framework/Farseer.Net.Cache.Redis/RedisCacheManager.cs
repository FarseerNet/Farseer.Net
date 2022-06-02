using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Cache.Redis.Configuration;
using FS.Core;
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
                var       server = _connectionWrapper.Server(endPoint: ep);
                using var keys   = server.Keys().ToPooledList();
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
        public void HashSetTransaction<TEntity, TEntityId>(string key, IEnumerable<TEntity> lst, Func<TEntity, TEntityId> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null)
        {
            if (lst == null || !lst.Any()) return;
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
        public Task HashSetTransactionAsync<TEntity, TEntityId>(string key, IEnumerable<TEntity> lst, Func<TEntity, TEntityId> funcDataKey, Func<TEntity, string> funcData = null, TimeSpan? expiry = null)
        {
            if (lst == null || !lst.Any()) return Task.FromResult(result: 0);
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

        /// <summary>
        /// 从Hash获取集合
        /// </summary>
        public IEnumerable<TEntity> HashToList<TEntity>(string key)
        {
            var hashGetAll = Db.HashGetAll(key);
            foreach (var hashEntry in hashGetAll)
            {
                var json = hashEntry.Value.ToString();
                yield return Jsons.ToObject<TEntity>(json);
            }
        }

        /// <summary>
        /// 从Hash获取集合
        /// </summary>
        public async IAsyncEnumerable<TEntity> HashToListAsync<TEntity>(string key)
        {
            var hashGetAll = await Db.HashGetAllAsync(key);
            foreach (var hashEntry in hashGetAll)
            {
                var json = hashEntry.Value.ToString();
                yield return Jsons.ToObject<TEntity>(json);
            }
        }

        /// <summary>
        /// 从Hash获取单个实体
        /// </summary>
        public TEntity HashToEntity<TEntity>(string key, string field) where TEntity : class
        {
            var hashEntry = Db.HashGet(key, field);
            return !hashEntry.HasValue ? null : Jsons.ToObject<TEntity>(hashEntry.ToString());
        }

        /// <summary>
        /// 从Hash获取单个实体
        /// </summary>
        public async Task<TEntity> HashToEntityAsync<TEntity>(string key, string field) where TEntity : class
        {
            var hashEntry = await Db.HashGetAsync(key, field);
            return !hashEntry.HasValue ? null : Jsons.ToObject<TEntity>(hashEntry.ToString());
        }
    }
}