using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Model;
using FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations;
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http
{
    public class MQConsumer
    {
        private readonly AliyunServiceClient _serviceClient;

        public MQConsumer(string instanceId, string topicName, string consumer, string messageTag, AliyunServiceClient serviceClient)
        {
            IntanceId      = instanceId;
            TopicName      = topicName;
            Consumer       = consumer;
            MessageTag     = messageTag;
            _serviceClient = serviceClient;
        }

        public string IntanceId { get; }

        public string TopicName { get; }

        public string Consumer { get; }

        public string MessageTag { get; }

        public bool IsSetInstance() => !string.IsNullOrEmpty(value: IntanceId);

        public bool IsSetTopicName() => TopicName != null;

        public bool IsSetConsumer() => Consumer != null;

        public bool IsSetMessageTag() => MessageTag != null;

        public AckMessageResponse AckMessage(List<string> receiptHandles)
        {
            var request = new AckMessageRequest(topicName: TopicName, consumer: Consumer, receiptHandles: receiptHandles);
            request.IntanceId = IntanceId;
            var marshaller   = new AckMessageRequestMarshaller();
            var unmarshaller = AckMessageResponseUnmarshaller.Instance;

            return _serviceClient.Invoke<AckMessageRequest, AckMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);
        }

        public List<Message> ConsumeMessage(uint batchSize)
        {
            var request = new ConsumeMessageRequest(topicName: TopicName, consumer: Consumer, messageTag: MessageTag);
            request.IntanceId = IntanceId;
            request.BatchSize = batchSize;
            var marshaller   = ConsumeMessageRequestMarshaller.Instance;
            var unmarshaller = ConsumeMessageResponseUnmarshaller.Instance;

            var result = _serviceClient.Invoke<ConsumeMessageRequest, ConsumeMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);

            return result.Messages;
        }

        public List<Message> ConsumeMessage(uint batchSize, uint waitSeconds)
        {
            var request = new ConsumeMessageRequest(topicName: TopicName, consumer: Consumer, messageTag: MessageTag);
            request.IntanceId   = IntanceId;
            request.BatchSize   = batchSize;
            request.WaitSeconds = waitSeconds;
            var marshaller   = ConsumeMessageRequestMarshaller.Instance;
            var unmarshaller = ConsumeMessageResponseUnmarshaller.Instance;

            var result = _serviceClient.Invoke<ConsumeMessageRequest, ConsumeMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);

            return result.Messages;
        }
    }
}