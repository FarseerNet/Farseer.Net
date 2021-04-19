using FS.MQ.RocketMQ.SDK;
using Message = FS.MQ.Rocket.SDK.Http.Model.Message;

namespace FS.MQ.Rocket
{
    public abstract class HttpMessageListener
    {
        public abstract Action Consume(Message message);
    }
}