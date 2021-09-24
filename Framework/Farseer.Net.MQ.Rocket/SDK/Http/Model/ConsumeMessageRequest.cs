using FS.MQ.Rocket.SDK.Http.Runtime.Internal;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class ConsumeMessageRequest : WebServiceRequest
    {
        private uint? _waitSeconds;

        public ConsumeMessageRequest(string topicName, string consumer)
        {
            TopicName = topicName;
            Consumer  = consumer;
        }

        public ConsumeMessageRequest(string topicName, string consumer, string messageTag)
        {
            TopicName  = topicName;
            Consumer   = consumer;
            MessageTag = messageTag;
        }

        public string TopicName { get; }

        public string Consumer { get; }

        public string MessageTag { get; }

        public uint WaitSeconds
        {
            get => _waitSeconds.GetValueOrDefault();
            set => _waitSeconds = value;
        }

        public uint BatchSize { get; set; }

        public string IntanceId { get; set; }

        internal string Trasaction { set; get; }

        public bool IsSetMessageTag() => MessageTag != null;

        public bool IsSetWaitSeconds() => _waitSeconds.HasValue;

        public bool IsSetInstance() => !string.IsNullOrEmpty(value: IntanceId);

        internal bool IsSetTransaction() => !string.IsNullOrEmpty(value: Trasaction);
    }
}