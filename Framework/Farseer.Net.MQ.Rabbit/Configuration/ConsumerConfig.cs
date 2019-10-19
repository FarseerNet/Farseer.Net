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
    }
}