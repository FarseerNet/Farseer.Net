using System.Net;
using FS.MQ.Rabbit.Configuration;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     Rabbit连接
    /// </summary>
    public class RabbitConnect
    {
        private readonly RabbitServerConfig _config;

        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private readonly IConnectionFactory _factoryInfo;

        /// <summary>
        ///     初始化
        /// </summary>
        /// <param name="config"> 配置 </param>
        public RabbitConnect(RabbitServerConfig config)
        {
            if (string.IsNullOrWhiteSpace(value: config.VirtualHost)) config.VirtualHost = "/";
            _config = config;
            _factoryInfo = new ConnectionFactory //创建连接工厂对象
            {
                HostName                 = config.Server,      //IP地址
                Port                     = config.Port,        //端口号
                UserName                 = config.UserName,    //用户账号
                Password                 = config.Password,    //用户密码
                VirtualHost              = config.VirtualHost, // 虚拟主机
                AutomaticRecoveryEnabled = true
            };
        }

        /// <summary>
        ///     Rabbit连接
        /// </summary>
        public IConnection Connection { get; private set; }

        /// <summary>
        ///     开启Rabbit
        /// </summary>
        public void Open()
        {
            var hostName = Dns.GetHostName();
            Connection = _factoryInfo.CreateConnection(clientProvidedName: $"{hostName}/{_config.UserName}");
        }

        /// <summary>
        ///     关闭Rabbit
        /// </summary>
        public void Close()
        {
            if (Connection != null) Connection.Close();
        }
    }
}