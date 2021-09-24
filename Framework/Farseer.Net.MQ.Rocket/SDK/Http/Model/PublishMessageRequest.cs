using FS.MQ.Rocket.SDK.Http.Runtime.Internal;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class PublishMessageRequest : WebServiceRequest
    {
        public PublishMessageRequest()
        {
        }

        public PublishMessageRequest(string messageBody)
            : this(messageBody: messageBody, messageTag: null)
        {
        }

        public PublishMessageRequest(string messageBody, string messageTag)
        {
            MessageBody = messageBody;
            MessageTag  = messageTag;
        }

        public string MessageBody { get; set; }

        public string MessageTag { get; set; }

        public string TopicName { get; set; }

        public string IntanceId { get; set; }

        public string Properties { get; set; }

        internal bool IsSetMessageBody() => MessageBody != null;

        internal bool IsSetMessageTag() => MessageTag != null;

        internal bool IsSetTopicName() => TopicName != null;

        public bool IsSetInstance() => !string.IsNullOrEmpty(value: IntanceId);

        public bool IsSetProperties() => !string.IsNullOrEmpty(value: Properties);
    }
}