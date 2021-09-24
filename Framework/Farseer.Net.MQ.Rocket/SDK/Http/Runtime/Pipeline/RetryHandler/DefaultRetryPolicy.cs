using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.RetryHandler
{
    public class DefaultRetryPolicy : RetryPolicy
    {
        // Set of error codes to retry on.

        // Set of web exception status codes to retry on.

        public DefaultRetryPolicy(int maxRetries)
        {
            MaxRetries = maxRetries;
        }

        public int MaxBackoffInMilliseconds { get; set; } = (int)TimeSpan.FromSeconds(value: 30).TotalMilliseconds;

        public ICollection<string> ErrorCodesToRetryOn { get; } = new HashSet<string>
        {
            "Throttling",
            "RequestTimeout"
        };

        public ICollection<WebExceptionStatus> WebExceptionStatusesToRetryOn { get; } = new HashSet<WebExceptionStatus>
        {
            WebExceptionStatus.ConnectFailure,
            WebExceptionStatus.ConnectionClosed,
            WebExceptionStatus.KeepAliveFailure,
            WebExceptionStatus.NameResolutionFailure,
            WebExceptionStatus.ReceiveFailure
        };

        public override bool CanRetry(IExecutionContext executionContext) => executionContext.RequestContext.Request.IsRequestStreamRewindable();

        public override bool RetryForException(IExecutionContext executionContext, Exception exception)
        {
            // An IOException was thrown by the underlying http client.
            if (exception is IOException)
            {
                // Don't retry IOExceptions that are caused by a ThreadAbortException
                if (IsInnerException<ThreadAbortException>(exception: exception)) return false;

                // Retry all other IOExceptions
                return true;
            }

            // A AliyunServiceException was thrown by ErrorHandler
            var serviceException = exception as AliyunServiceException;
            if (serviceException != null)
            {
                /*
                * For 500 internal server errors and 503 service
                * unavailable errors, we want to retry, but we need to use
                * an exponential back-off strategy so that we don't overload
                * a server with a flood of retries. If we've surpassed our
                * retry limit we handle the error response as a non-retryable
                * error and go ahead and throw it back to the user as an exception.
                */
                if (serviceException.StatusCode == HttpStatusCode.InternalServerError ||
                    serviceException.StatusCode == HttpStatusCode.ServiceUnavailable)
                    return true;

                /*
                 * Throttling is reported as a 400 or 503 error from services. To try and
                 * smooth out an occasional throttling error, we'll pause and retry,
                 * hoping that the pause is long enough for the request to get through
                 * the next time.
                */
                if (serviceException.StatusCode == HttpStatusCode.BadRequest ||
                    serviceException.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    var errorCode = serviceException.ErrorCode;
                    if (ErrorCodesToRetryOn.Contains(item: errorCode)) return true;
                }

                WebException webException;
                if (IsInnerException(exception: exception, inner: out webException))
                    if (WebExceptionStatusesToRetryOn.Contains(item: webException.Status))
                        return true;
            }

            return false;
        }

        public override bool RetryLimitReached(IExecutionContext executionContext) => executionContext.RequestContext.Retries >= MaxRetries;

        public override void WaitBeforeRetry(IExecutionContext executionContext)
        {
            WaitBeforeRetry(retries: executionContext.RequestContext.Retries, maxBackoffInMilliseconds: MaxBackoffInMilliseconds);
        }

        public static void WaitBeforeRetry(int retries, int maxBackoffInMilliseconds)
        {
            var delay = (int)(Math.Pow(x: 4, y: retries) * 100);
            delay = Math.Min(val1: delay, val2: maxBackoffInMilliseconds);
            AliyunSDKUtils.Sleep(ms: delay);
        }

        protected static bool IsInnerException<T>(Exception exception)
            where T : Exception
        {
            T innerException;
            return IsInnerException(exception: exception, inner: out innerException);
        }

        protected static bool IsInnerException<T>(Exception exception, out T inner)
            where T : Exception
        {
            inner = null;
            var innerExceptionType = typeof(T);
            var currentException   = exception;
            while (currentException.InnerException != null)
            {
                inner = currentException.InnerException as T;
                if (inner != null) return true;
                currentException = currentException.InnerException;
            }

            return false;
        }
    }
}