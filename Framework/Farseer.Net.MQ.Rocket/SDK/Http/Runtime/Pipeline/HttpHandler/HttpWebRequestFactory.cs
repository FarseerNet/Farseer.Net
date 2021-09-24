using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.HttpHandler
{
    /// <summary>
    ///     The request factory for System.Net.HttpWebRequest.
    /// </summary>
    public class HttpWebRequestFactory : IHttpRequestFactory<Stream>
    {
        /// <summary>
        ///     Creates an HTTP request for the given URI.
        /// </summary>
        /// <param name="requestUri"> The request URI. </param>
        /// <returns> An HTTP request. </returns>
        public IHttpRequest<Stream> CreateHttpRequest(Uri requestUri) => new HttpRequest(requestUri: requestUri);

        /// <summary>
        ///     Disposes the HttpWebRequestFactory.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }

    /// <summary>
    ///     HTTP request wrapper for System.Net.HttpWebRequest.
    /// </summary>
    public class HttpRequest : IHttpRequest<Stream>
    {
        private static readonly MethodInfo _addWithoutValidateHeadersMethod =
            typeof(WebHeaderCollection).GetMethod(name: "AddWithoutValidate", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

        private bool _isAborted;

        /// <summary>
        ///     Constructor for HttpRequest.
        /// </summary>
        /// <param name="requestUri"> The request URI. </param>
        public HttpRequest(Uri requestUri)
        {
            Request = WebRequest.Create(requestUri: requestUri) as HttpWebRequest;
        }

        /// <summary>
        ///     The underlying HTTP web request.
        /// </summary>
        public HttpWebRequest Request { get; }

        /// <summary>
        ///     The HTTP method or verb.
        /// </summary>
        public string Method
        {
            get => Request.Method;
            set => Request.Method = value;
        }

        /// <summary>
        ///     The request URI.
        /// </summary>
        public Uri RequestUri => Request.RequestUri;

        /// <summary>
        ///     Returns the HTTP response.
        /// </summary>
        /// <returns> The HTTP response. </returns>
        public virtual IWebResponseData GetResponse()
        {
            try
            {
                var response = Request.GetResponse() as HttpWebResponse;
                return new HttpWebRequestResponseData(response: response);
            }
            catch (WebException webException)
            {
                var errorResponse = webException.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    throw new HttpErrorResponseException(message: webException.Message,
                                                         innerException: webException,
                                                         response: new HttpWebRequestResponseData(response: errorResponse));
                }

                throw;
            }
        }

        /// <summary>
        ///     Gets a handle to the request content.
        /// </summary>
        /// <returns> The request content. </returns>
        public Stream GetRequestContent() => Request.GetRequestStream();

        /// <summary>
        ///     Writes a stream to the request body.
        /// </summary>
        /// <param name="requestContent"> The destination where the content stream is written. </param>
        /// <param name="contentStream"> The content stream to be written. </param>
        /// <param name="contentHeaders"> HTTP content headers. </param>
        /// <param name="requestContext"> The request context. </param>
        public void WriteToRequestBody
        (
            Stream                      requestContent,
            Stream                      contentStream,
            IDictionary<string, string> contentHeaders,
            IRequestContext             requestContext
        )
        {
            var gotException = false;
            try
            {
                var buffer      = new byte[requestContext.ClientConfig.BufferSize];
                var bytesRead   = 0;
                var bytesToRead = buffer.Length;

                while ((bytesRead = contentStream.Read(buffer: buffer, offset: 0, count: bytesToRead)) > 0) requestContent.Write(buffer: buffer, offset: 0, count: bytesRead);
            }
            catch (Exception)
            {
                gotException = true;

                // If an exception occured while reading the input stream,
                // Abort the request to signal failure to the server and prevent
                // potentially writing an incomplete stream to the server.
                Abort();
                throw;
            }
            finally
            {
                // Only bubble up exception from the close method if we haven't already got an exception
                // reading and writing from the streams.
                try
                {
                    requestContent.Close();
                }
                catch (Exception)
                {
                    if (!gotException) throw;
                }
            }
        }

        /// <summary>
        ///     Writes a byte array to the request body.
        /// </summary>
        /// <param name="requestContent"> The destination where the content stream is written. </param>
        /// <param name="content"> The content stream to be written. </param>
        /// <param name="contentHeaders"> HTTP content headers. </param>
        public void WriteToRequestBody(Stream requestContent, byte[] content, IDictionary<string, string> contentHeaders)
        {
            using (requestContent)
            {
                requestContent.Write(buffer: content, offset: 0, count: content.Length);
            }
        }

        /// <summary>
        ///     Aborts the HTTP request.
        /// </summary>
        public void Abort()
        {
            if (!_isAborted)
            {
                Request.Abort();
                _isAborted = true;
            }
        }

        /// <summary>
        ///     Initiates the operation to gets a handle to the request content.
        /// </summary>
        /// <param name="callback"> The async callback invoked when the operation completes. </param>
        /// <param name="state"> The state object to be passed to the async callback. </param>
        /// <returns> IAsyncResult that represents an async operation. </returns>
        public IAsyncResult BeginGetRequestContent(AsyncCallback callback, object state) => Request.BeginGetRequestStream(callback: callback, state: state);

        /// <summary>
        ///     Ends the operation to gets a handle to the request content.
        /// </summary>
        /// <param name="asyncResult"> IAsyncResult that represents an async operation. </param>
        /// <returns> The request content. </returns>
        public Stream EndGetRequestContent(IAsyncResult asyncResult) => Request.EndGetRequestStream(asyncResult: asyncResult);

        /// <summary>
        ///     Initiates the operation to Returns the HTTP response.
        /// </summary>
        /// <param name="callback"> The async callback invoked when the operation completes. </param>
        /// <param name="state"> The state object to be passed to the async callback. </param>
        /// <returns> IAsyncResult that represents an async operation. </returns>
        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state) => Request.BeginGetResponse(callback: callback, state: state);

        /// <summary>
        ///     Ends the operation to Returns the HTTP response.
        /// </summary>
        /// <param name="asyncResult"> IAsyncResult that represents an async operation. </param>
        /// <returns> The HTTP response. </returns>
        public virtual IWebResponseData EndGetResponse(IAsyncResult asyncResult)
        {
            try
            {
                var response = Request.EndGetResponse(asyncResult: asyncResult) as HttpWebResponse;
                return new HttpWebRequestResponseData(response: response);
            }
            catch (WebException webException)
            {
                var errorResponse = webException.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    throw new HttpErrorResponseException(message: webException.Message,
                                                         innerException: webException,
                                                         response: new HttpWebRequestResponseData(response: errorResponse));
                }

                throw;
            }
        }

        /// <summary>
        ///     Configures a request as per the request context.
        /// </summary>
        /// <param name="requestContext"> The request context. </param>
        public virtual void ConfigureRequest(IRequestContext requestContext)
        {
            var clientConfig    = requestContext.ClientConfig;
            var originalRequest = requestContext.OriginalRequest;

            // If System.Net.WebRequest.AllowAutoRedirect is set to true (default value),
            // redirects for GET requests are automatically followed and redirects for POST
            // requests are thrown back as exceptions.

            // If System.Net.WebRequest.AllowAutoRedirect is set to false,
            // redirects are returned as responses.
            Request.AllowAutoRedirect = clientConfig.AllowAutoRedirect;

            // Configure timeouts.
            if (requestContext.Request.ContentStream != null)
            {
                Request.Timeout                   = int.MaxValue;
                Request.ReadWriteTimeout          = int.MaxValue;
                Request.AllowWriteStreamBuffering = false;
            }

            // Override the Timeout and ReadWriteTimeout values if set at the request or config level.
            // Public Timeout and ReadWriteTimeout properties are present on client config objects.
            var timeout = ClientConfig.GetTimeoutValue(clientTimeout: clientConfig.Timeout,
                                                       requestTimeout: originalRequest.TimeoutInternal);
            var readWriteTimeout = ClientConfig.GetTimeoutValue(clientTimeout: clientConfig.ReadWriteTimeout,
                                                                requestTimeout: originalRequest.ReadWriteTimeoutInternal);
            if (timeout          != null) Request.Timeout          = (int)timeout.Value.TotalMilliseconds;
            if (readWriteTimeout != null) Request.ReadWriteTimeout = (int)readWriteTimeout.Value.TotalMilliseconds;
            Request.KeepAlive = originalRequest.KeepAlive;

            // Set proxy related properties
            if (!string.IsNullOrEmpty(value: requestContext.ClientConfig.ProxyHost) && requestContext.ClientConfig.ProxyPort > 0)
            {
                var proxy = new WebProxy(Host: requestContext.ClientConfig.ProxyHost, Port: requestContext.ClientConfig.ProxyPort);
                Request.Proxy = proxy;

                if (requestContext.ClientConfig.ProxyCredentials != null) Request.Proxy.Credentials = requestContext.ClientConfig.ProxyCredentials;
            }
            else
                Request.Proxy = null;

            // Set service point properties.
            //_request.ServicePoint.Expect100Continue = originalRequest.Expect100Continue;
        }

        /// <summary>
        ///     Sets the headers on the request.
        /// </summary>
        /// <param name="headers"> A dictionary of header names and values. </param>
        public void SetRequestHeaders(IDictionary<string, string> headers)
        {
            AddHeaders(request: Request, headersToAdd: headers);
        }

        /// <summary>
        ///     Disposes the HttpRequest.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        internal static void AddHeaders(HttpWebRequest request, IDictionary<string, string> headersToAdd)
        {
            var headers = request.Headers;
            foreach (var kvp in headersToAdd)
                if (WebHeaderCollection.IsRestricted(headerName: kvp.Key))
                {
                    if (string.Equals(a: kvp.Key, b: Constants.AcceptHeader, comparisonType: StringComparison.OrdinalIgnoreCase))
                        request.Accept = kvp.Value;
                    else if (string.Equals(a: kvp.Key, b: Constants.ContentTypeHeader, comparisonType: StringComparison.OrdinalIgnoreCase))
                        request.ContentType = kvp.Value;
                    else if (string.Equals(a: kvp.Key, b: Constants.ContentLengthHeader, comparisonType: StringComparison.OrdinalIgnoreCase))
                        request.ContentLength = long.Parse(s: kvp.Value, provider: CultureInfo.InvariantCulture);
                    else if (string.Equals(a: kvp.Key, b: Constants.UserAgentHeader, comparisonType: StringComparison.OrdinalIgnoreCase))
                        request.UserAgent = kvp.Value;
                    else if (string.Equals(a: kvp.Key, b: Constants.DateHeader, comparisonType: StringComparison.OrdinalIgnoreCase))
                        _addWithoutValidateHeadersMethod.Invoke(obj: request.Headers, parameters: new[] { Constants.DateHeader, kvp.Value });
                    else if (string.Equals(a: kvp.Key, b: Constants.HostHeader, comparisonType: StringComparison.OrdinalIgnoreCase))
                        _addWithoutValidateHeadersMethod.Invoke(obj: request.Headers, parameters: new[] { Constants.HostHeader, kvp.Value });
                    else
                        throw new NotSupportedException(message: "Header with name " + kvp.Key + " is not suppored");
                }
                else
                    headers[name: kvp.Key] = kvp.Value;
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}