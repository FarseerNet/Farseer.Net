using System.Collections.Generic;
using System.Linq;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class AckMessageRequest : WebServiceRequest
    {
        public AckMessageRequest(string topicName, string consumer, List<string> receiptHandles)
        {
            TopicName      = topicName;
            Consumer       = consumer;
            ReceiptHandles = receiptHandles;
        }

        public string TopicName { get; }

        public string Consumer { get; }

        public List<string> ReceiptHandles { get; set; } = new List<string>();

        public string IntanceId { get; set; }

        internal string Trasaction { set; get; }

        public bool IsSetReceiptHandles() => ReceiptHandles.Any();

        public bool IsSetInstance() => !string.IsNullOrEmpty(value: IntanceId);

        internal bool IsSetTransaction() => !string.IsNullOrEmpty(value: Trasaction);
    }
}