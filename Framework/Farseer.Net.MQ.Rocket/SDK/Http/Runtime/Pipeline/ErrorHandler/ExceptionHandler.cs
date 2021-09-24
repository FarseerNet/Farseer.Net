using System;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.ErrorHandler
{
    public abstract class ExceptionHandler<T> : IExceptionHandler<T> where T : Exception
    {
        public bool Handle(IExecutionContext executionContext, Exception exception) => HandleException(executionContext: executionContext, exception: exception as T);

        public abstract bool HandleException(IExecutionContext executionContext, T exception);
    }
}