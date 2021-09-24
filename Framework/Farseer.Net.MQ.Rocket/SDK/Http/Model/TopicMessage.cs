using System.Collections.Generic;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class TopicMessage
    {
        // only transaction msg have


        public TopicMessage(string body)
        {
            Body = body;
        }

        public TopicMessage(string body, string messageTag)
        {
            Body       = body;
            MessageTag = messageTag;
        }


        public string Id { get; set; }

        public string Body { get; }

        public string MessageTag { get; set; }

        public string BodyMD5 { get; set; }

        /// <summary>
        ///     发送事务消息的消息句柄，普通消息为空
        /// </summary>
        /// <value> The receipt handle. </value>
        public string ReceiptHandle { get; set; }

        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        ///     定时消息，单位毫秒（ms），在指定时间戳（当前时间之后）进行投递。如果被设置成当前时间戳之前的某个时刻，消息将立刻投递给消费者
        /// </summary>
        /// <param name="value"> Millis. </param>
        public long StartDeliverTime
        {
            set => Properties.Add(key: Constants.MESSAGE_PROPERTIES_TIMER_KEY, value: value.ToString());
        }

        /// <summary>
        ///     Sets the message key.
        /// </summary>
        /// <param name="value"> Key. </param>
        public string MessageKey
        {
            set => Properties.Add(key: Constants.MESSAGE_PROPERTIES_MSG_KEY, value: value);
        }

        /// <summary>
        ///     在消息属性中添加第一次消息回查的最快时间，单位秒，并且表征这是一条事务消息
        /// </summary>
        /// <value> The trans check immunity time. </value>
        public uint TransCheckImmunityTime
        {
            set => Properties.Add(key: Constants.MESSAGE_PROPERTIES_TRANS_CHECK_KEY, value: value.ToString());
        }

        // Check to see if Id property is set
        internal bool IsSetId() => Id != null;

        public bool IsSetBody() => Body != null;

        internal bool IsSetBodyMD5() => BodyMD5 != null;

        public void PutProperty(string key, string value)
        {
            Properties.Add(key: key, value: value);
        }

        internal bool IsSetReeceiptHandle() => !string.IsNullOrEmpty(value: ReceiptHandle);

        public override string ToString() => IsSetReeceiptHandle()
                                                 ? string.Format(format: "(MessageId {0}, MessageBodyMD5 {1}, Handle {2})", arg0: Id, arg1: BodyMD5, arg2: ReceiptHandle)
                                                 : string.Format(format: "(MessageId {0}, MessageBodyMD5 {1})", arg0: Id, arg1: BodyMD5);
    }
}