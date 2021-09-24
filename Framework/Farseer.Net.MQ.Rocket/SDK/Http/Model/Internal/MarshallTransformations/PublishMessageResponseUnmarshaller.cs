using System;
using System.Net;
using System.Xml;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations
{
    internal class PublishMessageResponseUnmarshaller : XmlResponseUnmarshaller
    {
        public static PublishMessageResponseUnmarshaller Instance { get; } = new PublishMessageResponseUnmarshaller();

        public override WebServiceResponse Unmarshall(XmlUnmarshallerContext context)
        {
            var reader   = new XmlTextReader(input: context.ResponseStream);
            var response = new PublishMessageResponse();

            while (reader.Read())
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.LocalName)
                        {
                            case Constants.XML_ELEMENT_MESSAGE_ID:
                                reader.Read();
                                response.MessageId = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_MESSAGE_BODY_MD5:
                                reader.Read();
                                response.MessageBodyMD5 = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_RECEIPT_HANDLE:
                                reader.Read();
                                response.ReeceiptHandle = reader.Value;
                                break;
                        }

                        break;
                }

            reader.Close();
            return response;
        }

        public override AliyunServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            var errorResponse = ErrorResponseUnmarshaller.Instance.Unmarshall(context: context);
            if (errorResponse.Code != null && errorResponse.Code.Equals(value: ErrorCode.TopicNotExist)) return new TopicNotExistException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
            if (errorResponse.Code != null && errorResponse.Code.Equals(value: ErrorCode.MalformedXML)) return new MalformedXMLException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
            if (errorResponse.Code != null && errorResponse.Code.Equals(value: ErrorCode.InvalidArgument)) return new InvalidArgumentException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
            return new MQException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
        }
    }
}