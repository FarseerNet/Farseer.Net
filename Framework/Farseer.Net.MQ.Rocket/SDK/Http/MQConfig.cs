
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http
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
