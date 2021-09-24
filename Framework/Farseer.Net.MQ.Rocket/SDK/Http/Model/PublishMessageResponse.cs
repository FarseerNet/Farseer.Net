using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http.Model
{
    public class PublishMessageResponse : WebServiceResponse
    {
        public string MessageBodyMD5 { get; set; }

        public string MessageId { get; set; }

        public string ReeceiptHandle { get; set; }

        // Check to see if BodyMD5 property is set
        internal bool IsSetMessageBodyMD5() => MessageBodyMD5 != null;

        // Check to see if MessageId property is set
        internal bool IsSetMessageId() => MessageId != null;

        internal bool IsSetReeceiptHandle() => !string.IsNullOrEmpty(value: ReeceiptHandle);

        public override string ToString() => IsSetReeceiptHandle()
                                                 ? string.Format(format: "(MessageId {0}, MessageBodyMD5 {1}, Handle {2})", arg0: MessageId, arg1: MessageBodyMD5, arg2: ReeceiptHandle)
                                                 : string.Format(format: "(MessageId {0}, MessageBodyMD5 {1})", arg0: MessageId, arg1: MessageBodyMD5);
    }
}