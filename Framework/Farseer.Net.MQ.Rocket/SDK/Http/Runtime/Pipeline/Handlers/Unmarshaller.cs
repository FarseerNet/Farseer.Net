using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.Handlers
{
    public class Unmarshaller : PipelineHandler
    {
        public override void InvokeSync(IExecutionContext executionContext)
        {
            base.InvokeSync(executionContext: executionContext);

            if (executionContext.ResponseContext.HttpResponse.IsSuccessStatusCode)
            {
                // Unmarshall the http response.
                Unmarshall(executionContext: executionContext);
            }
        }

        protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
        {
            // Unmarshall the response if an exception hasn't occured
            if (executionContext.ResponseContext.AsyncResult.Exception == null) Unmarshall(executionContext: ExecutionContext.CreateFromAsyncContext(asyncContext: executionContext));
            base.InvokeAsyncCallback(executionContext: executionContext);
        }

        private void Unmarshall(IExecutionContext executionContext)
        {
            var requestContext  = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;

            var unmarshaller = requestContext.Unmarshaller;
            try
            {
                var context = unmarshaller.CreateContext(response: responseContext.HttpResponse,
                                                         stream: responseContext.HttpResponse.ResponseBody.OpenResponse());

                var response = UnmarshallResponse(context: context, requestContext: requestContext);
                responseContext.Response = response;
            }
            finally
            {
                if (!unmarshaller.HasStreamingProperty) responseContext.HttpResponse.ResponseBody.Dispose();
            }
        }

        private WebServiceResponse UnmarshallResponse
        (
            UnmarshallerContext context,
            IRequestContext     requestContext
        )
        {
            var                unmarshaller = requestContext.Unmarshaller;
            WebServiceResponse response     = null;
            response = unmarshaller.UnmarshallResponse(context: context);

            return response;
        }
    }
}