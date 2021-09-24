using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    public class SubscriptionNotExistException : MQException
    {
        public SubscriptionNotExistException(string message)
            : base(message: message)
        {
        }

        public SubscriptionNotExistException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public SubscriptionNotExistException(Exception innerException)
            : base(innerException: innerException)
        {
        }

        public SubscriptionNotExistException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public SubscriptionNotExistException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}