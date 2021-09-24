using System;
using System.Net;
using FS.MQ.Rocket.SDK.Http.Runtime;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    public class MQException : AliyunServiceException
    {
        public MQException(string message)
            : base(message: message)
        {
        }

        public MQException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public MQException(Exception innerException)
            : base(message: innerException.Message, innerException: innerException)
        {
        }

        public MQException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public MQException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}