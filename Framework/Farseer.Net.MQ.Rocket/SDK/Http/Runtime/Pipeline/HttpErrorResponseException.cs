using System;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline
{
    /// <summary>
    ///     HttpErrorResponseException
    /// </summary>
    public class HttpErrorResponseException : Exception
    {
        public HttpErrorResponseException(IWebResponseData response)
        {
            Response = response;
        }

        public HttpErrorResponseException(string message, IWebResponseData response)
            : base(message: message)
        {
            Response = response;
        }

        public HttpErrorResponseException(string message, Exception innerException, IWebResponseData response)
            : base(message: message, innerException: innerException)
        {
            Response = response;
        }

        /// <summary>
        ///     Gets and sets original response data.
        /// </summary>
        public IWebResponseData Response { get; }
    }
}