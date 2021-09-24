using System;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Auth;

namespace FS.MQ.Rocket.SDK.Http
{
    public class MQClient : AliyunServiceClient
    {
        #region Overrides

        protected override IServiceSigner CreateSigner() => new MQSigner();

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing: disposing);
        }

        #endregion


        public MQProducer GetProducer(string instanceId, string topicName)
        {
            if (string.IsNullOrEmpty(value: topicName)) throw new MQException(message: "TopicName is null or empty");
            return new MQProducer(instanceId: instanceId, topicName: topicName, serviceClient: this);
        }

        public MQTransProducer GetTransProdcuer(string instanceId, string topicName, string groupId)
        {
            if (string.IsNullOrEmpty(value: topicName)) throw new MQException(message: "TopicName is null or empty");

            if (string.IsNullOrEmpty(value: groupId)) throw new MQException(message: "TopicName is null or empty");

            return new MQTransProducer(instanceId: instanceId, topicName: topicName, groupId: groupId, serviceClient: this);
        }

        public MQConsumer GetConsumer(string instanceId, string topicName, string consumer, string messageTag)
        {
            if (string.IsNullOrEmpty(value: topicName)) throw new MQException(message: "TopicName is null or empty");
            if (string.IsNullOrEmpty(value: consumer)) throw new MQException(message: "Consumer is null or empty");
            return new MQConsumer(instanceId: instanceId, topicName: topicName, consumer: consumer, messageTag: messageTag, serviceClient: this);
        }

        #region Constructors

        public MQClient(string accessKeyId, string secretAccessKey, string regionEndpoint)
            : base(accessKeyId: accessKeyId, secretAccessKey: secretAccessKey, config: new MQConfig { RegionEndpoint = new Uri(uriString: regionEndpoint) }, stsToken: null)
        {
        }

        public MQClient(string accessKeyId, string secretAccessKey, string regionEndpoint, string stsToken)
            : base(accessKeyId: accessKeyId, secretAccessKey: secretAccessKey, config: new MQConfig { RegionEndpoint = new Uri(uriString: regionEndpoint) }, stsToken: stsToken)
        {
        }

        #endregion
    }
}