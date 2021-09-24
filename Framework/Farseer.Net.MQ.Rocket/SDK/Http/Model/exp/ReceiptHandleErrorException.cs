using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Model.exp
{
    /// <summary>
    ///     ReceiptHandleErrorException
    /// </summary>
    public class ReceiptHandleErrorException : MQException
    {
        /// <summary>
        ///     Constructs a new ReceiptHandleErrorException with the specified error message.
        /// </summary>
        public ReceiptHandleErrorException(string message)
            : base(message: message)
        {
        }

        public ReceiptHandleErrorException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public ReceiptHandleErrorException(Exception innerException)
            : base(innerException: innerException)
        {
        }

        public ReceiptHandleErrorException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }

        public ReceiptHandleErrorException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException, errorCode: errorCode, requestId: requestId, hostId: hostId, statusCode: statusCode)
        {
        }
    }
}