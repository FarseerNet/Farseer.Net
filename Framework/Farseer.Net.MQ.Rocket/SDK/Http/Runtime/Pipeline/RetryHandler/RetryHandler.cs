using System;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    ///     The retry handler has the generic logic for retrying requests.
    ///     It uses a retry policy which specifies when
    ///     a retry should be performed.
    /// </summary>
    public class RetryHandler : PipelineHandler
    {
        /// <summary>
        ///     Constructor which takes in a retry policy.
        /// </summary>
        /// <param name="retryPolicy"> Retry Policy </param>
        public RetryHandler(RetryPolicy retryPolicy)
        {
            RetryPolicy = retryPolicy;
        }

        /// <summary>
        ///     The retry policy which specifies when
        ///     a retry should be performed.
        /// </summary>
        public RetryPolicy RetryPolicy { get; }

        /// <summary>
        ///     Invokes the inner handler and performs a retry, if required as per the
        ///     retry policy.
        /// </summary>
        /// <param name="executionContext">
        ///     The execution context which contains both the
        ///     requests and response context.
        /// </param>
        public override void InvokeSync(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var shouldRetry    = false;
            do
            {
                try
                {
                    base.InvokeSync(executionContext: executionContext);
                    try
                    {
                        if (requestContext.Request.ContentStream != null) requestContext.Request.ContentStream.Close();
                    }
                    catch (Exception)
                    {
                    }

                    return;
                }
                catch (Exception exception)
                {
                    shouldRetry = RetryPolicy.Retry(executionContext: executionContext, exception: exception);
                    if (!shouldRetry) throw;
                    requestContext.Retries++;
                }

                PrepareForRetry(requestContext: requestContext);

                RetryPolicy.WaitBeforeRetry(executionContext: executionContext);
            }
            while (shouldRetry);
        }

        /// <summary>
        ///     Invokes the inner handler and performs a retry, if required as per the
        ///     retry policy.
        /// </summary>
        /// <param name="executionContext">
        ///     The execution context which contains both the
        ///     requests and response context.
        /// </param>
        protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
        {
            var requestContext  = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;
            var exception       = responseContext.AsyncResult.Exception;

            if (exception != null)
            {
                var syncExecutionContext = ExecutionContext.CreateFromAsyncContext(asyncContext: executionContext);
                var shouldRetry          = RetryPolicy.Retry(executionContext: syncExecutionContext, exception: exception);
                if (shouldRetry)
                {
                    requestContext.Retries++;

                    PrepareForRetry(requestContext: requestContext);

                    RetryPolicy.WaitBeforeRetry(executionContext: syncExecutionContext);

                    // Retry by calling InvokeAsync
                    InvokeAsync(executionContext: executionContext);
                    return;
                }
            }

            // Call outer handler
            base.InvokeAsyncCallback(executionContext: executionContext);
        }

        /// <summary>
        ///     Prepares the request for retry.
        /// </summary>
        /// <param name="requestContext"> Request context containing the state of the request. </param>
        internal static void PrepareForRetry(IRequestContext requestContext)
        {
            if (requestContext.Request.ContentStream          != null &&
                requestContext.Request.OriginalStreamPosition >= 0)
            {
                var stream = requestContext.Request.ContentStream;
                stream.Position = requestContext.Request.OriginalStreamPosition;
            }
        }
    }
}