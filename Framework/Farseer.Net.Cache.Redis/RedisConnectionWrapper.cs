using System;
using System.Net;
using StackExchange.Redis;
using Farseer.Net.Cache.Redis.Configuration;

namespace Farseer.Net.Cache.Redis
{
    /// <summary>
    ///     Redis连接包装器
    /// </summary>
    public class RedisConnectionWrapper : IRedisConnectionWrapper
    {
        private readonly RedisItemConfig _config;
        private readonly Lazy<string> _connectionString;
        private volatile ConnectionMultiplexer _connection;
        private readonly object _lock = new object();

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="config">配置</param>
        public RedisConnectionWrapper(RedisItemConfig config)
        {
            _config = config;
            _connectionString = new Lazy<string>(GetConnectionString);
        }

        /// <summary>
        ///     获取数据库连接字符串
        /// </summary>
        /// <returns>数据库连接字符串</returns>
        private string GetConnectionString() => _config.Server;

        /// <summary>
        ///     获取连接
        /// </summary>
        /// <returns>ConnectionMultiplexer</returns>
        private ConnectionMultiplexer GetConnection()
        {
            if ((_connection != null) && _connection.IsConnected) return _connection;

            lock (_lock)
            {
                if ((_connection != null) && _connection.IsConnected) return _connection;

                _connection?.Dispose();
                _connection = ConnectionMultiplexer.Connect(_connectionString.Value);
            }

            return _connection;
        }

        /// <summary>
        ///     获取数据库
        /// </summary>
        /// <param name="db">数据库编号</param>
        /// <returns>IDatabase</returns>
        public IDatabase Database(int? db = null) { return GetConnection().GetDatabase(db ?? -1); //_settings.DefaultDb);
        }

        /// <summary>
        ///     获取服务器
        /// </summary>
        /// <param name="endPoint">终结点</param>
        /// <returns>IServer</returns>
        public IServer Server(EndPoint endPoint) { return GetConnection().GetServer(endPoint); }

        /// <summary>
        ///     获取终结点
        /// </summary>
        /// <returns>终结点数组</returns>
        public EndPoint[] GetEndpoints() { return GetConnection().GetEndPoints(); }

        /// <summary>
        ///     清空数据库
        /// </summary>
        /// <param name="db">数据库编号</param>
        public void FlushDb(int? db = null)
        {
            var endPoints = GetEndpoints();

            foreach (var endPoint in endPoints) Server(endPoint).FlushDatabase(db ?? -1); //_settings.DefaultDb);
        }

        /// <summary>
        ///     清理资源
        /// </summary>
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}