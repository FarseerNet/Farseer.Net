using RabbitMQ.Client.Events;

namespace FS.MQ.RabbitMQ
{
    /// <summary>
    /// Rabbit监听消费
    /// </summary>
    public interface IListenerMessage
    {
         /// <summary>
         /// 消费
         /// </summary>
         /// <param name="message"></param>
         /// <param name="sender"></param>
         /// <param name="ea"></param>
         /// <returns>当开启手动确认时，返回true时，才会进行ACK确认</returns>
         bool Consumer(string message, object sender, BasicDeliverEventArgs ea);
    }
}