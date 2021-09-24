using System;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline
{
    public abstract class PipelineHandler : IPipelineHandler
    {
        public IPipelineHandler InnerHandler { get; set; }

        public IPipelineHandler OuterHandler { get; set; }

        public virtual void InvokeSync(IExecutionContext executionContext)
        {
            if (InnerHandler != null)
            {
                InnerHandler.InvokeSync(executionContext: executionContext);
                return;
            }

            throw new InvalidOperationException(message: "Cannot invoke InnerHandler. InnerHandler is not set.");
        }

        public virtual IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
        {
            if (InnerHandler != null) return InnerHandler.InvokeAsync(executionContext: executionContext);
            throw new InvalidOperationException(message: "Cannot invoke InnerHandler. InnerHandler is not set.");
        }

        public void AsyncCallback(IAsyncExecutionContext executionContext)
        {
            try
            {
                InvokeAsyncCallback(executionContext: executionContext);
            }
            catch (Exception exception)
            {
                // An unhandled exception occured in the callback implementation.
                // Capture the exception and end the callback processing by signalling the
                // wait handle.

                var asyncResult = executionContext.ResponseContext.AsyncResult;
                asyncResult.Exception = exception;
                asyncResult.SignalWaitHandle();
                if (asyncResult.AsyncCallback != null) asyncResult.AsyncCallback(ar: asyncResult);
            }
        }

        protected virtual void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
        {
            if (OuterHandler != null)
                OuterHandler.AsyncCallback(executionContext: executionContext);
            else
            {
                // No more outer handlers to process, signal completion
                executionContext.ResponseContext.AsyncResult.Response =
                    executionContext.ResponseContext.Response;

                var asyncResult = executionContext.ResponseContext.AsyncResult;
                asyncResult.SignalWaitHandle();
                if (asyncResult.AsyncCallback != null) asyncResult.AsyncCallback(ar: asyncResult);
            }
        }
    }
}