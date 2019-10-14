
using FS.MQ.RocketMQ.SDK.Http.Runtime;

namespace FS.MQ.RocketMQ.SDK.Http
{
    public partial class MQConfig : ClientConfig
    {
        public MQConfig()
        {
        }

        public override string ServiceVersion
        {
            get
            {
                return "2015-06-06";
            }
        }

        public override string ServiceName
        {
            get
            {
                return "Aliyun.MQ";
            }
        }
    }
}
