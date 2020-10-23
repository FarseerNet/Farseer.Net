using System;
using System.Net;
using StackExchange.Redis;
using FS.Cache.Redis.Configuration;
using FS.Extends;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis连接包装器
    /// </summary>
    public class RedisConnectionWrapper : IRedisConnectionWrapper
    {
        private readonly RedisItemConfig _config;
        private volatile ConnectionMultiplexer _connection;
        private readonly object _lock = new object();

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="config">配置</param>
        public RedisConnectionWrapper(RedisItemConfig config) => _config = config;

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

                var option = new ConfigurationOptions();
                // 模式选择
                switch (_config.CommandType)
                {
                    case EumCommandType.Default:
                        option.CommandMap = CommandMap.Default;
                        break;
                    case EumCommandType.Sentinel:
                        option.ServiceName = _config.ServiceName;
                        option.CommandMap = CommandMap.Sentinel;
                        option.TieBreaker = _config.TieBreaker;
                        option.DefaultVersion = new Version(3, 0, 6);
                        option.AllowAdmin = true;
                        ConfigurationOptions masterConfig = new ConfigurationOptions();
                        // 

                        if (string.IsNullOrWhiteSpace(_config.ServiceName)) throw new FarseerException($"Redis哨兵模式下，需要为{_config.Name}配置ServiceName");
                        break;
                    case EumCommandType.Twemproxy:
                        option.CommandMap = CommandMap.Twemproxy;
                        option.Proxy = Proxy.Twemproxy;
                        break;
                    case EumCommandType.SSDB:
                        option.CommandMap = CommandMap.SSDB;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                option.Password = _config.Password;
                foreach (var endPoints in _config.Server.Split(','))
                {
                    if (endPoints.Contains(":")) option.EndPoints.Add(endPoints.Split(':')[0], endPoints.Split(':')[1].ConvertType(0));
                    else
                    {
                        var timeout = endPoints.ToLower().Split('=');
                        if (timeout.Length != 2) continue;
                        var time = timeout[1].ConvertType(0);
                        switch (timeout[0])
                        {
                            case "synctimeout": option.SyncTimeout = time; break;
                            //case "asynctimeout": option.AsyncTimeout = time; break;
                            case "connecttimeout": option.ConnectTimeout = time; break;
                            //case "responsetimeout": option.ResponseTimeout = time; break;
                        }
                    }
                }
                _connection = ConnectionMultiplexer.Connect(option);
            }

            var isConnected = _connection.GetSubscriber().IsConnected();
            _connection.GetSubscriber().Subscribe("+switch-master", (channel, message) =>
            {
                Console.WriteLine((string)message);
            });
            return _connection;
        }

        /// <summary>
        ///     获取数据库
        /// </summary>
        /// <param name="db">数据库编号</param>
        /// <returns>IDatabase</returns>
        public IDatabase Database(int? db = null) => GetConnection().GetDatabase(db ?? -1);

        /// <summary>
        ///     获取服务器
        /// </summary>
        /// <param name="endPoint">终结点</param>
        /// <returns>IServer</returns>
        public IServer Server(EndPoint endPoint) => GetConnection().GetServer(endPoint);

        /// <summary>
        ///     获取终结点
        /// </summary>
        /// <returns>终结点数组</returns>
        public EndPoint[] GetEndpoints() => GetConnection().GetEndPoints();

        /// <summary>
        ///     清空数据库
        /// </summary>
        /// <param name="db">数据库编号</param>
        public void FlushDb(int? db = null)
        {
            var endPoints = GetEndpoints();

            foreach (var endPoint in endPoints) Server(endPoint).FlushDatabase(db ?? -1);
        }

        /// <summary>
        ///     清理资源
        /// </summary>
        public void Dispose() => _connection?.Dispose();
    }
}