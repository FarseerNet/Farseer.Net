using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    /// <summary>
    ///     TopicNotExistException
    /// </summary>
    public class TopicNotExistException : MQException
    {
        /// <summary>
        ///     Constructs a new TopicNotExistException with the specified error message.
        /// </summary>
        public TopicNotExistException(string message)
            : base(message: message)
        {
        }

        public TopicNotExistException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public TopicNotExistException(Exception innerException)
            : base(innerException: innerException)
        {
        }

        public TopicNotExistException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public TopicNotExistException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}