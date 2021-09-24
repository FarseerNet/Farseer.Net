using System;
using System.Globalization;
using System.Text;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.HttpHandler
{
    /// <summary>
    ///     The HTTP handler contains common logic for issuing an HTTP request that is
    ///     independent of the underlying HTTP infrastructure.
    /// </summary>
    /// <typeparam name="TRequestContent"> </typeparam>
    public class HttpHandler<TRequestContent> : PipelineHandler, IDisposable
    {
        private          bool                                 _disposed;
        private readonly IHttpRequestFactory<TRequestContent> _requestFactory;

        /// <summary>
        ///     The constructor for HttpHandler.
        /// </summary>
        /// <param name="requestFactory"> The request factory used to create HTTP Requests. </param>
        /// <param name="callbackSender"> The sender parameter used in any events raised by this handler. </param>
        public HttpHandler(IHttpRequestFactory<TRequestContent> requestFactory, object callbackSender)
        {
            _requestFactory = requestFactory;
            CallbackSender  = callbackSender;
        }

        /// <summary>
        ///     The sender parameter used in any events raised by this handler.
        /// </summary>
        public object CallbackSender { get; }

        /// <summary>
        ///     Disposes the HTTP handler.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        /// <summary>
        ///     Issues an HTTP request for the current request context.
        /// </summary>
        /// <param name="executionContext">
        ///     The execution context which contains both the
        ///     requests and response context.
        /// </param>
        public override void InvokeSync(IExecutionContext executionContext)
        {
            IHttpRequest<TRequestContent> httpRequest = null;
            try
            {
                var wrappedRequest = executionContext.RequestContext.Request;
                httpRequest = CreateWebRequest(requestContext: executionContext.RequestContext);
                httpRequest.SetRequestHeaders(headers: wrappedRequest.Headers);

                // Send request body if present.
                if (wrappedRequest.HasRequestBody())
                {
                    var requestContent = httpRequest.GetRequestContent();
                    WriteContentToRequestBody(requestContent: requestContent, httpRequest: httpRequest, requestContext: executionContext.RequestContext);
                }

                executionContext.ResponseContext.HttpResponse = httpRequest.GetResponse();
            }
            finally
            {
                if (httpRequest != null) httpRequest.Dispose();
            }
        }

        /// <summary>
        ///     Issues an HTTP request for the current request context.
        /// </summary>
        /// <param name="executionContext">
        ///     The execution context which contains both the
        ///     requests and response context.
        /// </param>
        /// <returns> IAsyncResult which represent an async operation. </returns>
        public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
        {
            IHttpRequest<TRequestContent> httpRequest = null;
            try
            {
                httpRequest                   = CreateWebRequest(requestContext: executionContext.RequestContext);
                executionContext.RuntimeState = httpRequest;

                var wrappedRequest = executionContext.RequestContext.Request;
                if (executionContext.RequestContext.Retries == 0)
                {
                    // First call, initialize an async result.
                    executionContext.ResponseContext.AsyncResult =
                        new RuntimeAsyncResult(asyncCallback: executionContext.RequestContext.Callback,
                                               asyncState: executionContext.RequestContext.State);
                }

                // Set request headers
                httpRequest.SetRequestHeaders(headers: executionContext.RequestContext.Request.Headers);

                if (wrappedRequest.HasRequestBody())
                {
                    // Send request body if present.
                    httpRequest.BeginGetRequestContent(callback: GetRequestStreamCallback, state: executionContext);
                }
                else
                {
                    // Get response if there is no response body to send.
                    httpRequest.BeginGetResponse(callback: GetResponseCallback, state: executionContext);
                }

                return executionContext.ResponseContext.AsyncResult;
            }
            catch (Exception)
            {
                if (executionContext.ResponseContext.AsyncResult != null)
                {
                    // An exception will be thrown back to the calling code.
                    // Dispose AsyncResult as it will not be used further.
                    executionContext.ResponseContext.AsyncResult.Dispose();
                    executionContext.ResponseContext.AsyncResult = null;
                }

                if (httpRequest != null) httpRequest.Dispose();

                throw;
            }
        }

        private void GetRequestStreamCallback(IAsyncResult result)
        {
            IAsyncExecutionContext        executionContext = null;
            IHttpRequest<TRequestContent> httpRequest      = null;
            try
            {
                executionContext = result.AsyncState as IAsyncExecutionContext;
                httpRequest      = executionContext.RuntimeState as IHttpRequest<TRequestContent>;

                var requestContent = httpRequest.EndGetRequestContent(asyncResult: result);
                WriteContentToRequestBody(requestContent: requestContent, httpRequest: httpRequest, requestContext: executionContext.RequestContext);
                //var requestStream = httpRequest.EndSetRequestBody(result);                
                httpRequest.BeginGetResponse(callback: GetResponseCallback, state: executionContext);
            }
            catch (Exception exception)
            {
                httpRequest.Dispose();

                // Capture the exception and invoke outer handlers to 
                // process the exception.
                executionContext.ResponseContext.AsyncResult.Exception = exception;
                base.InvokeAsyncCallback(executionContext: executionContext);
            }
        }

        private void GetResponseCallback(IAsyncResult result)
        {
            IAsyncExecutionContext        executionContext = null;
            IHttpRequest<TRequestContent> httpRequest      = null;
            try
            {
                executionContext = result.AsyncState as IAsyncExecutionContext;
                httpRequest      = executionContext.RuntimeState as IHttpRequest<TRequestContent>;

                var httpResponse = httpRequest.EndGetResponse(asyncResult: result);
                executionContext.ResponseContext.HttpResponse = httpResponse;
            }
            catch (Exception exception)
            {
                // Capture the exception and invoke outer handlers to 
                // process the exception.
                executionContext.ResponseContext.AsyncResult.Exception = exception;
            }
            finally
            {
                httpRequest.Dispose();
                base.InvokeAsyncCallback(executionContext: executionContext);
            }
        }

        /// <summary>
        ///     Determines the content for request body and uses the HTTP request
        ///     to write the content to the HTTP request body.
        /// </summary>
        /// <param name="requestContent"> Content to be written. </param>
        /// <param name="httpRequest"> The HTTP request. </param>
        /// <param name="requestContext"> The request context. </param>
        private void WriteContentToRequestBody
        (
            TRequestContent               requestContent,
            IHttpRequest<TRequestContent> httpRequest,
            IRequestContext               requestContext
        )
        {
            var wrappedRequest = requestContext.Request;

            if (wrappedRequest.ContentStream == null)
            {
                var requestData = wrappedRequest.Content;
                httpRequest.WriteToRequestBody(requestContent: requestContent, content: requestData, contentHeaders: requestContext.Request.Headers);
            }
            else
            {
                var originalStream = wrappedRequest.ContentStream;
                httpRequest.WriteToRequestBody(requestContent: requestContent, contentStream: originalStream,
                                               contentHeaders: requestContext.Request.Headers, requestContext: requestContext);
            }
        }

        /// <summary>
        ///     Creates the HttpWebRequest and configures the end point, content, user agent and proxy settings.
        /// </summary>
        /// <param name="requestContext"> The async request. </param>
        /// <returns> The web request that actually makes the call. </returns>
        protected virtual IHttpRequest<TRequestContent> CreateWebRequest(IRequestContext requestContext)
        {
            var request     = requestContext.Request;
            var url         = AliyunServiceClient.ComposeUrl(iRequest: request);
            var httpRequest = _requestFactory.CreateHttpRequest(requestUri: url);
            httpRequest.ConfigureRequest(requestContext: requestContext);

            httpRequest.Method = request.HttpMethod;
            if (request.MayContainRequestBody())
            {
                if (request.Content == null && request.ContentStream == null)
                {
                    var queryString = AliyunSDKUtils.GetParametersAsString(parameters: request.Parameters);
                    request.Content = Encoding.UTF8.GetBytes(s: queryString);
                }

                if (request.Content != null)
                {
                    request.Headers[key: Constants.ContentLengthHeader] =
                        request.Content.Length.ToString(provider: CultureInfo.InvariantCulture);
                }
                else if (request.ContentStream != null && !request.Headers.ContainsKey(key: Constants.ContentLengthHeader))
                {
                    request.Headers[key: Constants.ContentLengthHeader] =
                        request.ContentStream.Length.ToString(provider: CultureInfo.InvariantCulture);
                }
            }
            else if (request.UseQueryString &&
                     (request.HttpMethod == "POST" ||
                      request.HttpMethod == "PUT"  ||
                      request.HttpMethod == "DELETE"))
                request.Content = new byte[0];

            return httpRequest;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_requestFactory != null) _requestFactory.Dispose();

                _disposed = true;
            }
        }
    }
}