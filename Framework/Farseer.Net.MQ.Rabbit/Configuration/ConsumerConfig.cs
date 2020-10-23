namespace FS.MQ.RabbitMQ.Configuration
{
    public class ConsumerConfig
    {
        /// <summary> 配置名称 </summary>
        public string Name { get; set; }

        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }

        /// <summary> 线程数 </summary>
        public int ConsumeThreadNums { get; set; }

        /// <summary> 最后ACK多少秒超时则重连（默认5分钟） </summary>
        public int LastAckTimeoutRestart { get; set; }
    }
}