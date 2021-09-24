using System;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime
{
    public class AliyunServiceException : Exception
    {
        public AliyunServiceException()
        {
        }

        public AliyunServiceException(string message)
            : base(message: message)
        {
        }

        public AliyunServiceException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        public AliyunServiceException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException)
        {
            this.StatusCode = statusCode;
        }

        public AliyunServiceException(Exception innerException)
            : base(message: innerException.Message, innerException: innerException)
        {
        }

        public AliyunServiceException(string message, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message)
        {
            this.StatusCode = statusCode;
            this.ErrorCode  = errorCode;
            this.RequestId  = requestId;
            this.HostId     = hostId;
        }

        public AliyunServiceException(string message, Exception innerException, string errorCode, string requestId, string hostId, HttpStatusCode statusCode)
            : base(message: message, innerException: innerException)
        {
            this.StatusCode = statusCode;
            this.ErrorCode  = errorCode;
            this.RequestId  = requestId;
            this.HostId     = hostId;
        }

        /// <summary>
        ///     The HTTP status code from the service response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        ///     The error code returned by the service
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        ///     The id of the request which generated the exception.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        ///     The host id of the request which generated the exception.
        /// </summary>
        public string HostId { get; set; }
    }
}