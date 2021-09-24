using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    public class InvalidArgumentException : MQException
    {
        /// <summary>
        ///     Constructs a new InvalidArgumentException with the specified error message.
        /// </summary>
        public InvalidArgumentException(string message)
            : base(message: message)
        {
        }

        public InvalidArgumentException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public InvalidArgumentException(Exception innerException)
            : base(innerException: innerException)
        {
        }

        public InvalidArgumentException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public InvalidArgumentException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}