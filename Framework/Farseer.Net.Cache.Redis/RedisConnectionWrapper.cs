using System;
using System.Net;
using FS.Cache.Redis.Configuration;
using FS.Extends;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis连接包装器
    /// </summary>
    public class RedisConnectionWrapper : IRedisConnectionWrapper
    {
        private readonly RedisItemConfig       _config;
        private readonly object                _lock = new();
        private volatile ConnectionMultiplexer _connection;
        private          int                   _dbIndex;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="config"> 配置 </param>
        public RedisConnectionWrapper(RedisItemConfig config)
        {
            _config = config;
        }

        /// <summary>
        ///     获取数据库
        /// </summary>
        /// <param name="db"> 数据库编号 </param>
        /// <returns> IDatabase </returns>
        public IDatabase Database(int? db = null) => GetConnection().GetDatabase(db: db ?? _dbIndex);

        /// <summary>
        ///     获取服务器
        /// </summary>
        /// <param name="endPoint"> 终结点 </param>
        /// <returns> IServer </returns>
        public IServer Server(EndPoint endPoint) => GetConnection().GetServer(endpoint: endPoint);

        /// <summary>
        ///     获取终结点
        /// </summary>
        /// <returns> 终结点数组 </returns>
        public EndPoint[] GetEndpoints() => GetConnection().GetEndPoints();

        /// <summary>
        ///     清空数据库
        /// </summary>
        /// <param name="db"> 数据库编号 </param>
        public void FlushDb(int? db = null)
        {
            var endPoints = GetEndpoints();

            foreach (var endPoint in endPoints) Server(endPoint: endPoint).FlushDatabase(database: db ?? _dbIndex);
        }

        /// <summary>
        ///     清理资源
        /// </summary>
        public void Dispose() => _connection?.Dispose();

        /// <summary>
        ///     获取连接
        /// </summary>
        /// <returns> ConnectionMultiplexer </returns>
        private ConnectionMultiplexer GetConnection()
        {
            if (_connection is { IsConnected: true }) return _connection;

            lock (_lock)
            {
                if (_connection is { IsConnected: true }) return _connection;
                _connection?.Dispose();

                var option = new ConfigurationOptions
                {
                    Password = _config.Password
                };

                // 模式选择
                switch (_config.CommandType)
                {
                    case EumCommandType.Default:
                        option.CommandMap = CommandMap.Default;
                        break;
                    case EumCommandType.Sentinel:
                        option.ServiceName    = _config.ServiceName;
                        option.CommandMap     = CommandMap.Sentinel;
                        option.TieBreaker     = _config.TieBreaker;
                        option.DefaultVersion = new Version(major: 3, minor: 0, build: 6);
                        option.AllowAdmin     = true;
                        if (string.IsNullOrWhiteSpace(value: _config.ServiceName)) throw new FarseerException(message: $"Redis哨兵模式下，需要为{_config.Name}配置ServiceName");
                        break;
                    case EumCommandType.Twemproxy:
                        option.CommandMap = CommandMap.Twemproxy;
                        option.Proxy      = Proxy.Twemproxy;
                        break;
                    case EumCommandType.SSDB:
                        option.CommandMap = CommandMap.SSDB;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                foreach (var endPoints in _config.Server.Split(','))
                {
                    if (!endPoints.Contains(":")) continue;
                    option.EndPoints.Add(host: endPoints.Split(':')[0], port: endPoints.Split(':')[1].ConvertType(defValue: 0));
                }
                _dbIndex = _config.DB;
                if (_config.SyncTimeout     > 0) option.SyncTimeout     = _config.SyncTimeout;
                if (_config.ConnectTimeout  > 0) option.ConnectTimeout  = _config.ConnectTimeout;
                _connection = ConnectionMultiplexer.Connect(configuration: option);
            }
            return _connection;
        }
    }
}