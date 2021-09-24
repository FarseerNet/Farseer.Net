using System;
using System.Collections.Generic;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime
{
    /// <summary>
    ///     Abstract class for Response objects, contains response headers,
    ///     and no result information.
    /// </summary>
    public class WebServiceResponse
    {
        /// <summary>
        ///     Returns the headers of the HTTP response.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Returns the content length of the HTTP response.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        ///     Returns the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}