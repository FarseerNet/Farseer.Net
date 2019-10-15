namespace FS.MQ.RabbitMQ.Configuration
{
    public class RabbitServerConfig
    {
        /// <summary> 用户名 </summary>
        public string UserName { get; set; }

        /// <summary> 密码 </summary>
        public string Password { get; set; }

        /// <summary> 集群地址 </summary>
        public string Server { get; set; }

        /// <summary> 端口 </summary>
        public int Port { get; set; }
    }
}