using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket.Configuration
{
    /// <summary>
    ///     RocketMQ配置信息
    /// </summary>
    public class RocketItemConfig
    {
        /// <summary> 配置名称 </summary>
        public string Name { get; set; }

        /// <summary> 鉴权用AccessKey </summary>
        public string AccessKey { get; set; }

        /// <summary> 鉴权用SecretKey </summary>
        public string SecretKey { get; set; }

        /// <summary> 消费者ID </summary>
        public string ConsumerID { get; set; }

        /// <summary> 生产者ID </summary>
        public string ProducerID { get; set; }

        /// <summary> 实例ID </summary>
        public string InstanceID { get; set; }

        /// <summary> 消息队列主题名称 </summary>
        public string Topic { get; set; }

        /// <summary> 集群地址,多个地址用;隔开 </summary>
        public string Server { get; set; }

        /// <summary> 聚石塔用户必须设置为CLOUD，阿里云用户不需要设置 </summary>
        public ONSChannel Channel { get; set; }

        /// <summary> 消费端的线程数 </summary>
        public int ConsumeThreadNums { get; set; }

        /// <summary> 是否输出日志 </summary>
        public bool IsWriteLog { get; set; }

        /// <summary> Http消费长轮询等待时间(最多可设置为30秒) </summary>
        public int HttpConsumeWaitSeconds { get; set; }
    }
}