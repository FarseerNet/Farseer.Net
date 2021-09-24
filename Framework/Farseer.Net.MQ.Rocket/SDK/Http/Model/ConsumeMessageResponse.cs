using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class ConsumeMessageResponse : WebServiceResponse
    {
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}