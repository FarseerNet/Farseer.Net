using System;
using System.Net;
using System.Xml;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations
{
    internal class ConsumeMessageResponseUnmarshaller : XmlResponseUnmarshaller
    {
        public static ConsumeMessageResponseUnmarshaller Instance { get; } = new ConsumeMessageResponseUnmarshaller();

        public override WebServiceResponse Unmarshall(XmlUnmarshallerContext context)
        {
            var     reader                 = new XmlTextReader(input: context.ResponseStream);
            var     consumeMessageResponse = new ConsumeMessageResponse();
            Message message                = null;

            while (reader.Read())
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.LocalName)
                        {
                            case Constants.XML_ROOT_MESSAGE:
                                message = new Message();
                                break;
                            case Constants.XML_ELEMENT_MESSAGE_ID:
                                reader.Read();
                                message.Id = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_RECEIPT_HANDLE:
                                reader.Read();
                                message.ReceiptHandle = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_MESSAGE_BODY_MD5:
                                reader.Read();
                                message.BodyMD5 = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_MESSAGE_BODY:
                                reader.Read();
                                message.Body = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_PUBLISH_TIME:
                                reader.Read();
                                message.PublishTime = long.Parse(s: reader.Value);
                                break;
                            case Constants.XML_ELEMENT_NEXT_CONSUME_TIME:
                                reader.Read();
                                message.NextConsumeTime = long.Parse(s: reader.Value);
                                break;
                            case Constants.XML_ELEMENT_FIRST_CONSUME_TIME:
                                reader.Read();
                                message.FirstConsumeTime = long.Parse(s: reader.Value);
                                break;
                            case Constants.XML_ELEMENT_CONSUMED_TIMES:
                                reader.Read();
                                message.ConsumedTimes = uint.Parse(s: reader.Value);
                                break;
                            case Constants.XML_ELEMENT_MESSAGE_TAG:
                                reader.Read();
                                message.MessageTag = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_MESSAGE_PROPERTIES:
                                reader.Read();
                                AliyunSDKUtils.StringToDict(str: reader.Value, dict: message.Properties);
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == Constants.XML_ROOT_MESSAGE) consumeMessageResponse.Messages.Add(item: message);
                        break;
                }

            reader.Close();
            return consumeMessageResponse;
        }

        public override AliyunServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            var errorResponse = ErrorResponseUnmarshaller.Instance.Unmarshall(context: context);
            if (errorResponse.Code != null && errorResponse.Code.Equals(value: ErrorCode.TopicNotExist)) return new TopicNotExistException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
            if (errorResponse.Code != null && errorResponse.Code.Equals(value: ErrorCode.SubscriptionNotExist)) return new SubscriptionNotExistException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
            if (errorResponse.Code != null && errorResponse.Code.Equals(value: ErrorCode.MessageNotExist)) return new MessageNotExistException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
            return new MQException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
        }
    }
}