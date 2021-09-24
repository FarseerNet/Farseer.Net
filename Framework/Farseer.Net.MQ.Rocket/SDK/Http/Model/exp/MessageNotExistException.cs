using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    /// <summary>
    ///     MessageNotExistException
    /// </summary>
    public class MessageNotExistException : MQException
    {
        /// <summary>
        ///     Constructs a new MessageNotExistException with the specified error message.
        /// </summary>
        public MessageNotExistException(string message)
            : base(message: message)
        {
        }

        public MessageNotExistException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public MessageNotExistException(Exception innerException)
            : base(innerException: innerException)
        {
        }

        public MessageNotExistException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public MessageNotExistException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}