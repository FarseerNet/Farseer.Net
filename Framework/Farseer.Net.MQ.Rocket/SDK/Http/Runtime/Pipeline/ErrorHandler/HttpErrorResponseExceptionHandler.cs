using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using FS.MQ.Rocket.SDK.Http.Model.exp;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.ErrorHandler
{
    /// <summary>
    ///     The exception handler for HttpErrorResponseException exception.
    /// </summary>
    public class HttpErrorResponseExceptionHandler : ExceptionHandler<HttpErrorResponseException>
    {
        /// <summary>
        ///     Handles an exception for the given execution context.
        /// </summary>
        /// <param name="executionContext">
        ///     The execution context, it contains the
        ///     request and response context.
        /// </param>
        /// <param name="exception"> The exception to handle. </param>
        /// <returns>
        ///     Returns a boolean value which indicates if the original exception
        ///     should be rethrown.
        ///     This method can also throw a new exception to replace the original exception.
        /// </returns>
        public override bool HandleException(IExecutionContext executionContext, HttpErrorResponseException exception)
        {
            var requestContext    = executionContext.RequestContext;
            var httpErrorResponse = exception.Response;

            // If 404 was suppressed and successfully unmarshalled,
            // don't rethrow the original exception.
            if (HandleSuppressed404(executionContext: executionContext, httpErrorResponse: httpErrorResponse)) return false;

            AliyunServiceException errorResponseException = null;
            // Unmarshall the service error response and throw the corresponding service exception
            string responseContent = null;
            try
            {
                using (httpErrorResponse.ResponseBody)
                {
                    var unmarshaller = requestContext.Unmarshaller;

                    var errorContext = unmarshaller.CreateContext(response: httpErrorResponse, stream: httpErrorResponse.ResponseBody.OpenResponse());

                    using (var stream = new MemoryStream())
                    {
                        AliyunSDKUtils.CopyTo(input: errorContext.ResponseStream, output: stream);
                        stream.Seek(offset: 0, loc: SeekOrigin.Begin);
                        var bytes = new byte[stream.Length];
                        stream.Read(buffer: bytes, offset: 0, count: (int)stream.Length);
                        responseContent = Encoding.UTF8.GetString(bytes: bytes);
                        stream.Seek(offset: 0, loc: SeekOrigin.Begin);

                        errorContext.ResponseStream = stream;

                        errorResponseException = unmarshaller.UnmarshallException(input: errorContext,
                                                                                  innerException: exception, statusCode: httpErrorResponse.StatusCode);
                        Debug.Assert(condition: errorResponseException != null);
                    }
                }
            }
            catch (ResponseUnmarshallException unmarshallException)
            {
                if (responseContent != null)
                {
                    throw new AliyunServiceException(message: responseContent, innerException: unmarshallException,
                                                     errorCode: ErrorCode.InternalError,
                                                     requestId: null, hostId: null, statusCode: httpErrorResponse.StatusCode);
                }

                throw;
            }

            throw errorResponseException;
        }

        /// <summary>
        ///     Checks if a HTTP 404 status code is returned which needs to be suppressed and
        ///     processes it.
        ///     If a suppressed 404 is present, it unmarshalls the response and returns true to
        ///     indicate that a suppressed 404 was processed, else returns false.
        /// </summary>
        /// <param name="executionContext">
        ///     The execution context, it contains the
        ///     request and response context.
        /// </param>
        /// <param name="httpErrorResponse"> </param>
        /// <returns>
        ///     If a suppressed 404 is present, returns true, else returns false.
        /// </returns>
        private bool HandleSuppressed404(IExecutionContext executionContext, IWebResponseData httpErrorResponse)
        {
            var requestContext  = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;

            // If the error is a 404 and the request is configured to supress it,
            // then unmarshall as much as we can.
            if (httpErrorResponse            != null                    &&
                httpErrorResponse.StatusCode == HttpStatusCode.NotFound &&
                requestContext.Request.Suppress404Exceptions)
            {
                using (httpErrorResponse.ResponseBody)
                {
                    var unmarshaller = requestContext.Unmarshaller;

                    var errorContext = unmarshaller.CreateContext(
                                                                  response: httpErrorResponse,
                                                                  stream: httpErrorResponse.ResponseBody.OpenResponse());
                    try
                    {
                        responseContext.Response                = unmarshaller.Unmarshall(input: errorContext);
                        responseContext.Response.ContentLength  = httpErrorResponse.ContentLength;
                        responseContext.Response.HttpStatusCode = httpErrorResponse.StatusCode;
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return false;
        }
    }
}