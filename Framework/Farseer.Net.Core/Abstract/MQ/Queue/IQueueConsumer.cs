namespace FS.Core.Abstract.MQ.Queue
{
    public interface IQueueConsumer
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
    }
}