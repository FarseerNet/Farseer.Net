using System;
using System.Net;
using System.Xml;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using FS.MQ.Rocket.SDK.Http.Runtime;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Model.Internal.MarshallTransformations
{
    public class AckMessageResponseUnmarshaller : XmlResponseUnmarshaller
    {
        public static AckMessageResponseUnmarshaller Instance { get; } = new AckMessageResponseUnmarshaller();

        public override WebServiceResponse Unmarshall(XmlUnmarshallerContext context) => new AckMessageResponse();

        public override AliyunServiceException UnmarshallException(XmlUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            var reader = new XmlTextReader(input: context.ResponseStream);

            var errorResponse = new ErrorResponse();
            while (reader.Read())
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.LocalName == Constants.XML_ROOT_ERROR_RESPONSE)
                            return UnmarshallNormalError(reader: reader, innerException: innerException, statusCode: statusCode);
                        else
                        {
                            var ackMessageException = UnmarshallAckMessageError(reader: reader);
                            ackMessageException.RequestId = context.ResponseData.GetHeaderValue(headerName: "x-mq-request-id");
                            return ackMessageException;
                        }
                }

            return new MQException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
        }

        private AckMessageException UnmarshallAckMessageError(XmlTextReader reader)
        {
            var                 ackMessageException = new AckMessageException();
            AckMessageErrorItem item                = null;

            while (reader.Read())
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.LocalName)
                        {
                            case Constants.XML_ROOT_ERROR_RESPONSE:
                                item = new AckMessageErrorItem();
                                break;
                            case Constants.XML_ELEMENT_ERROR_CODE:
                                reader.Read();
                                item.ErrorCode = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_ERROR_MESSAGE:
                                reader.Read();
                                item.ErrorMessage = reader.Value;
                                break;
                            case Constants.XML_ELEMENT_RECEIPT_HANDLE:
                                reader.Read();
                                item.ReceiptHandle = reader.Value;
                                break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == Constants.XML_ROOT_ERROR_RESPONSE) ackMessageException.ErrorItems.Add(item: item);
                        break;
                }

            reader.Close();
            return ackMessageException;
        }

        private AliyunServiceException UnmarshallNormalError(XmlTextReader reader, Exception innerException, HttpStatusCode statusCode)
        {
            var errorResponse = ErrorResponseUnmarshaller.Instance.Unmarshall(reader: reader);
            if (errorResponse.Code != null)
            {
                switch (errorResponse.Code)
                {
                    case ErrorCode.SubscriptionNotExist: return new SubscriptionNotExistException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
                    case ErrorCode.TopicNotExist:        return new TopicNotExistException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
                    case ErrorCode.InvalidArgument:      return new InvalidArgumentException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
                    case ErrorCode.ReceiptHandleError:   return new ReceiptHandleErrorException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
                }
            }

            return new MQException(message: errorResponse.Message, innerException: innerException, errorCode: errorResponse.Code, requestId: errorResponse.RequestId, hostId: errorResponse.HostId, statusCode: statusCode);
        }
    }
}