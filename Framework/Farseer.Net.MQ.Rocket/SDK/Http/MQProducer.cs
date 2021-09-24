using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Model;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http
{
    public class MQProducer
    {
        protected readonly AliyunServiceClient _serviceClient;
        protected          string              _instanceId;

        protected string _topicName;

        public MQProducer(string instanceId, string topicName, AliyunServiceClient serviceClient)
        {
            _instanceId    = instanceId;
            _topicName     = topicName;
            _serviceClient = serviceClient;
        }

        public string TopicName => _topicName;

        public string IntanceId => _instanceId;

        public bool IsSetTopicName() => _topicName != null;

        public bool IsSetInstance() => !string.IsNullOrEmpty(value: _instanceId);

        public TopicMessage PublishMessage(TopicMessage topicMessage)
        {
            var request = new PublishMessageRequest(messageBody: topicMessage.Body, messageTag: topicMessage.MessageTag);
            request.TopicName  = _topicName;
            request.IntanceId  = _instanceId;
            request.Properties = AliyunSDKUtils.DictToString(dict: topicMessage.Properties);

            var marshaller   = PublishMessageRequestMarshaller.Instance;
            var unmarshaller = PublishMessageResponseUnmarshaller.Instance;

            var result = _serviceClient.Invoke<PublishMessageRequest, PublishMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);

            var retMsg = new TopicMessage(body: null);
            retMsg.Id            = result.MessageId;
            retMsg.BodyMD5       = result.MessageBodyMD5;
            retMsg.ReceiptHandle = result.ReeceiptHandle;

            return retMsg;
        }
    }

    public class MQTransProducer : MQProducer
    {
        public MQTransProducer(string instanceId, string topicName, string groupId, AliyunServiceClient serviceClient) : base(instanceId: instanceId, topicName: topicName, serviceClient: serviceClient)
        {
            if (string.IsNullOrEmpty(value: groupId)) throw new MQException(message: "GroupId is null or empty!");
            GroupId = groupId;
        }

        public string GroupId { get; }

        /// <summary>
        ///     commit transaction msg, the consumer will receive the msg.
        /// </summary>
        /// <returns> The commit. </returns>
        /// <param name="receiptHandle"> Receipt handle. </param>
        public AckMessageResponse Commit(string receiptHandle)
        {
            var handlers = new List<string>
            {
                receiptHandle
            };

            var request = new AckMessageRequest(topicName: _topicName, consumer: GroupId, receiptHandles: handlers);
            request.IntanceId  = _instanceId;
            request.Trasaction = "commit";
            var marshaller   = new AckMessageRequestMarshaller();
            var unmarshaller = AckMessageResponseUnmarshaller.Instance;

            return _serviceClient.Invoke<AckMessageRequest, AckMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);
        }

        /// <summary>
        ///     rollback transaction msg, the consumer will not receive the msg.
        /// </summary>
        /// <returns> The rollback. </returns>
        /// <param name="receiptHandle"> Receipt handle. </param>
        public AckMessageResponse Rollback(string receiptHandle)
        {
            var handlers = new List<string>
            {
                receiptHandle
            };

            var request = new AckMessageRequest(topicName: _topicName, consumer: GroupId, receiptHandles: handlers);
            request.IntanceId  = _instanceId;
            request.Trasaction = "rollback";
            var marshaller   = new AckMessageRequestMarshaller();
            var unmarshaller = AckMessageResponseUnmarshaller.Instance;

            return _serviceClient.Invoke<AckMessageRequest, AckMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);
        }

        /// <summary>
        ///     Consumes the half tranaction message.
        /// </summary>
        /// <returns> The half message. </returns>
        /// <param name="batchSize"> Batch size. 1~16 </param>
        /// <param name="waitSeconds"> Wait seconds. 1~30 is valid, others will be ignored. </param>
        public List<Message> ConsumeHalfMessage(uint batchSize, uint waitSeconds)
        {
            var request = new ConsumeMessageRequest(topicName: _topicName, consumer: GroupId, messageTag: null);
            request.IntanceId   = _instanceId;
            request.BatchSize   = batchSize;
            request.WaitSeconds = waitSeconds;
            request.Trasaction  = "pop";
            var marshaller   = ConsumeMessageRequestMarshaller.Instance;
            var unmarshaller = ConsumeMessageResponseUnmarshaller.Instance;

            var result = _serviceClient.Invoke<ConsumeMessageRequest, ConsumeMessageResponse>(request: request, marshaller: marshaller, unmarshaller: unmarshaller);

            return result.Messages;
        }
    }
}