using System;
using System.Collections.Generic;
using System.Net;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.ErrorHandler
{
    public class ErrorHandler : PipelineHandler
    {
        public ErrorHandler()
        {
            ExceptionHandlers = new Dictionary<Type, IExceptionHandler>
            {
                { typeof(WebException), new WebExceptionHandler() },
                { typeof(HttpErrorResponseException), new HttpErrorResponseExceptionHandler() }
            };
        }

        public IDictionary<Type, IExceptionHandler> ExceptionHandlers { get; }

        public override void InvokeSync(IExecutionContext executionContext)
        {
            try
            {
                base.InvokeSync(executionContext: executionContext);
            }
            catch (Exception exception)
            {
                DisposeReponse(responseContext: executionContext.ResponseContext);
                var rethrowOriginalException = ProcessException(executionContext: executionContext, exception: exception);
                if (rethrowOriginalException) throw;
            }
        }

        protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
        {
            // Unmarshall the response if an exception hasn't occured
            var exception = executionContext.ResponseContext.AsyncResult.Exception;
            if (exception != null)
            {
                DisposeReponse(responseContext: executionContext.ResponseContext);
                try
                {
                    var rethrowOriginalException = ProcessException(executionContext: ExecutionContext.CreateFromAsyncContext(asyncContext: executionContext),
                                                                    exception: exception);
                    if (rethrowOriginalException) executionContext.ResponseContext.AsyncResult.Exception = exception;
                }
                catch (Exception ex)
                {
                    executionContext.ResponseContext.AsyncResult.Exception = ex;
                }
            }

            base.InvokeAsyncCallback(executionContext: executionContext);
        }

        private static void DisposeReponse(IResponseContext responseContext)
        {
            if (responseContext.HttpResponse              != null &&
                responseContext.HttpResponse.ResponseBody != null)
                responseContext.HttpResponse.ResponseBody.Dispose();
        }

        private bool ProcessException(IExecutionContext executionContext, Exception exception)
        {
            // Find the matching handler which can process the exception
            // Start by checking if there is a matching handler for the specific exception type,
            // if not check for handlers for it's base type till we find a match.
            var exceptionType = exception.GetType();
            do
            {
                IExceptionHandler exceptionHandler = null;

                if (ExceptionHandlers.TryGetValue(key: exceptionType, value: out exceptionHandler)) return exceptionHandler.Handle(executionContext: executionContext, exception: exception);
            }
            while (exceptionType != typeof(Exception));

            // No match found, rethrow the original exception.
            return true;
        }
    }
}