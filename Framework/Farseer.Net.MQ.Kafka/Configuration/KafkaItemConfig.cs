namespace FS.MQ.Kafka.Configuration
{
    /// <summary>
    /// Kafka集群配置信息
    /// </summary>
    public class KafkaItemConfig
    {
        /// <summary> 配置名称 </summary>
        public string Name { get; set; }

        /// <summary> MQ地址 </summary>
        public string Server { get; set; }

        /// <summary>
        /// 消费者的Group名称，需要指定
        /// </summary>
        public string GroupID { get; set; }

        /// <summary>
        /// 自动提交Offset
        /// </summary>
        public bool? EnabledAutoCommit { get; set; }

        /// <summary>
        /// 自动提交时间
        /// </summary>
        public int? AutoCommitIntervalMS { get; set; }

        /// <summary>
        ///  时间间隔
        /// </summary>
        public int? StatisticsIntervalMS { get; set; }


        /// <summary>
        /// offset自动重置
        /// </summary>
        public  string AutoOffetReset { get; set; }


    }
}
