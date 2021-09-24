using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http
{
    public class MQConfig : ClientConfig
    {
        public override string ServiceVersion => "2015-06-06";

        public override string ServiceName => "Aliyun.MQ";
    }
}