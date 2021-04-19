using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public partial class ConsumeMessageResponse : WebServiceResponse
    {
        private List<Message> _messages = new List<Message>();

        public List<Message> Messages
        {
            get { return this._messages; }
            set { this._messages = value; }
        }
    }
}
