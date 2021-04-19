namespace FS.MQ.Rabbit
{
    public enum eumExchangeType
    {
        /// <summary>
        /// 默认交换器（集群模式）
        /// </summary>
        direct,
        /// <summary>
        /// 
        /// </summary>
        headers,
        /// <summary>
        /// 消息广播到所有消费者
        /// </summary>
        fanout,
        /// <summary>
        /// 消息广播到所有消费者（并支持匹配队列）
        /// </summary>
        topic
    }
}