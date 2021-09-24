using System;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.Handlers
{
    public abstract class GenericHandler : PipelineHandler
    {
        public override void InvokeSync(IExecutionContext executionContext)
        {
            PreInvoke(executionContext: executionContext);
            base.InvokeSync(executionContext: executionContext);
            PostInvoke(executionContext: executionContext);
        }

        public override IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
        {
            PreInvoke(executionContext: ExecutionContext.CreateFromAsyncContext(asyncContext: executionContext));
            return base.InvokeAsync(executionContext: executionContext);
        }

        protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
        {
            PostInvoke(executionContext: ExecutionContext.CreateFromAsyncContext(asyncContext: executionContext));
            base.InvokeAsyncCallback(executionContext: executionContext);
        }

        protected virtual void PreInvoke(IExecutionContext executionContext)
        {
        }

        protected virtual void PostInvoke(IExecutionContext executionContext)
        {
        }
    }
}