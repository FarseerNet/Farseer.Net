namespace FS.MQ.Rabbit.Configuration
{
    public class RabbitServerConfig
    {
        /// <summary>
        /// Connect配置名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary> 用户名 </summary>
        public string UserName { get; set; }

        /// <summary> 密码 </summary>
        public string Password { get; set; }

        /// <summary> 集群地址 </summary>
        public string Server { get; set; }

        /// <summary> 端口 </summary>
        public int Port { get; set; }

        /// <summary> 虚拟主机 </summary>
        public string VirtualHost { get; set; }
    }
}