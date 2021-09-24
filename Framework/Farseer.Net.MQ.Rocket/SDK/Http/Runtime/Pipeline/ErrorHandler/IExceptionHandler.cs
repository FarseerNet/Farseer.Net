using System;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.ErrorHandler
{
    public interface IExceptionHandler
    {
        bool Handle(IExecutionContext executionContext, Exception exception);
    }

    public interface IExceptionHandler<T> : IExceptionHandler where T : Exception
    {
        bool HandleException(IExecutionContext executionContext, T exception);
    }
}