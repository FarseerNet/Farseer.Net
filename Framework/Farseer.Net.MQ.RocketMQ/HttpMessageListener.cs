using FS.MQ.RocketMQ.SDK;
using Message = FS.MQ.RocketMQ.SDK.Http.Model.Message;

namespace FS.MQ.RocketMQ
{
    public abstract class HttpMessageListener
    {
        public abstract Action Consume(Message message);
    }
}