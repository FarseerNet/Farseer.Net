using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    public class MalformedXMLException : MQException
    {
        /// <summary>
        ///     Constructs a new MalformedXMLException with the specified error message.
        /// </summary>
        public MalformedXMLException(string message)
            : base(message: message)
        {
        }

        public MalformedXMLException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public MalformedXMLException(Exception innerException)
            : base(innerException: innerException)
        {
        }

        public MalformedXMLException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public MalformedXMLException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}