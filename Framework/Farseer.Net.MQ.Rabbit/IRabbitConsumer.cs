namespace FS.MQ.Rabbit
{
    public interface IRabbitConsumer
    {
        /// <summary>
        ///     关闭生产者
        /// </summary>
        void Close();

        /// <summary>
        ///     监控消费
        /// </summary>
        /// <param name="listener"> 消费事件 </param>
        /// <param name="autoAck"> 是否自动确认，默认false </param>
        void Start(IListenerMessage listener, bool autoAck = false);

        /// <summary>
        ///     监控消费（只消费一次）
        /// </summary>
        /// <param name="listener"> 消费事件 </param>
        /// <param name="queueName"> 队列名称 </param>
        /// <param name="autoAck"> 是否自动确认，默认false </param>
        void StartSignle(IListenerMessageBatch listener, bool autoAck = false);
    }
}