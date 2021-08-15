using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public IDatabase Db
        {
            get { return _db ??= new LinkTrackWarp(_connectionWrapper.Database()); }
        }

        /// <summary>
        /// 支持缓存不存在，则写入
        /// </summary>
        public ICacheManager CacheManager { get; internal set; }

        /// <summary>
        /// Redis服务端
        /// </summary>
        public string Server { get; }

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="connectionWrapper">Redis连接</param>
        public RedisCacheManager(RedisItemConfig config, IRedisConnectionWrapper connectionWrapper)
        {
            Check.NotNull(config.Server, "Redis连接字符串为空");
            Server             = config.Server;
            _connectionWrapper = connectionWrapper;
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
            var tasks       = new List<Task>();
            foreach (var po in lst)
            {
                var dataKey = funcDataKey(po).ToString();
                var data    = funcData(po);
                tasks.Add(transaction.HashSetAsync(key, dataKey, data));
            }

            transaction.Execute();
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        ///     事务，批量写入HASH
        /// </summary>
        public Task HashSetTransactionAsync<TEntity>(string key, List<TEntity> lst, Func<TEntity, object> funcDataKey, Func<TEntity, string> funcData = null)
        {
            if (lst == null || lst.Count == 0) return Task.FromResult(0);
            if (funcData == null) funcData = po => JsonConvert.SerializeObject(po);

            var transaction = Db.CreateTransaction();
            foreach (var po in lst)
            {
                var dataKey = funcDataKey(po).ToString();
                var data    = funcData(po);
                transaction.HashSetAsync(key, dataKey, data);
            }

            return transaction.ExecuteAsync();
        }
    }
}