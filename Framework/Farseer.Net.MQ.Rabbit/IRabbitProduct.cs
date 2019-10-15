using RabbitMQ.Client;

namespace FS.MQ.RabbitMQ
{
    public interface IRabbitProduct
    {
        /// <summary>
        ///     关闭生产者
        /// </summary>
        void Close();

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="basicProperties">属性</param>
        bool Send(string message, IBasicProperties basicProperties = null);

        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="message">消息主体</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchange">交换器名称</param>
        /// <param name="basicProperties">属性</param>
        bool Send(string message, string queueName, string exchange = "", IBasicProperties basicProperties = null);
    }
}