namespace FS.MQ.RabbitMQ.Configuration
{
    /// <summary>
    ///     RocketMQ配置信息
    /// </summary>
    public class RabbitItemConfig
    {
        /// <summary> 配置名称 </summary>
        public string Name { get; set; }

        /// <summary> 用户名 </summary>
        public string UserName { get; set; }

        /// <summary> 密码 </summary>
        public string Password { get; set; }

        /// <summary> 集群地址 </summary>
        public string Server { get; set; }

        /// <summary> 端口 </summary>
        public int Port { get; set; }

        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }

        /// <summary> 交换器名称 </summary>
        public string ExchangeName { get; set; }

        /// <summary> 使用确认模式（默认关闭） </summary>
        public bool UseConfirmModel { get; set; }
//        
//        
//
//        /// <summary> 消费端的线程数 </summary>
//        public int ConsumeThreadNums { get; set; }
//
//        /// <summary> 是否输出日志 </summary>
//        public bool IsWriteLog { get; set; }
//
//        /// <summary> Http消费长轮询等待时间(最多可设置为30秒) </summary>
//        public int HttpConsumeWaitSeconds { get; set; }
    }
}