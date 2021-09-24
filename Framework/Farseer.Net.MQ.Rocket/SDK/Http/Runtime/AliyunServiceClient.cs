using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Auth;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;
using FS.MQ.Rocket.SDK.Http.Runtime.Pipeline;
using FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.ErrorHandler;
using FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.Handlers;
using FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.HttpHandler;
using FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.RetryHandler;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime
{
    public abstract class AliyunServiceClient : IDisposable
    {
        private bool _disposed;

        protected RuntimePipeline    RuntimePipeline { get; set; }
        protected ServiceCredentials Credentials     { get; }
        internal  ClientConfig       Config          { get; }

        protected abstract IServiceSigner CreateSigner();

        protected virtual void CustomizeRuntimePipeline(RuntimePipeline pipeline)
        {
        }

        private void BuildRuntimePipeline()
        {
            var httpRequestFactory = new HttpWebRequestFactory();
            var httpHandler        = new HttpHandler<Stream>(requestFactory: httpRequestFactory, callbackSender: this);

            RuntimePipeline = new RuntimePipeline(handlers: new List<IPipelineHandler>
                                                  {
                                                      httpHandler,
                                                      new Unmarshaller(),
                                                      new ErrorHandler(),
                                                      new Signer(),
                                                      new CredentialsRetriever(credentials: Credentials),
                                                      new RetryHandler(retryPolicy: new DefaultRetryPolicy(maxRetries: Config.MaxErrorRetry)),
                                                      new Marshaller()
                                                  }
                                                 );

            CustomizeRuntimePipeline(pipeline: RuntimePipeline);
        }

        internal static Uri ComposeUrl(IRequest iRequest)
        {
            var url          = iRequest.Endpoint;
            var resourcePath = iRequest.ResourcePath;
            if (resourcePath == null)
                resourcePath = string.Empty;
            else
            {
                if (resourcePath.StartsWith(value: "//", comparisonType: StringComparison.Ordinal))
                    resourcePath                                                                                     = resourcePath.Substring(startIndex: 2);
                else if (resourcePath.StartsWith(value: "/", comparisonType: StringComparison.Ordinal)) resourcePath = resourcePath.Substring(startIndex: 1);
            }

            var delim = "?";
            var sb    = new StringBuilder();

            if (iRequest.SubResources.Count > 0)
            {
                foreach (var subResource in iRequest.SubResources)
                {
                    sb.AppendFormat(format: "{0}{1}", arg0: delim, arg1: subResource.Key);
                    if (subResource.Value != null) sb.AppendFormat(format: "={0}", arg0: subResource.Value);
                    delim = "&";
                }
            }

            if (iRequest.Parameters.Count > 0)
            {
                var queryString = AliyunSDKUtils.GetParametersAsString(parameters: iRequest.Parameters);
                sb.AppendFormat(format: "{0}{1}", arg0: delim, arg1: queryString);
            }

            var parameterizedPath = string.Concat(arg0: resourcePath, arg1: sb);
            var uri               = new Uri(uriString: url.AbsoluteUri + parameterizedPath);
            return uri;
        }

        #region Constructors

        internal AliyunServiceClient(ServiceCredentials credentials, ClientConfig config)
        {
            ServicePointManager.Expect100Continue       = true;
            ServicePointManager.DefaultConnectionLimit  = config.ConnectionLimit;
            ServicePointManager.MaxServicePointIdleTime = config.MaxIdleTime;

            Config      = config;
            Credentials = credentials;
            Signer      = CreateSigner();

            Initialize();

            BuildRuntimePipeline();
        }

        protected IServiceSigner Signer { get; }

        internal AliyunServiceClient(string accessKeyId, string secretAccessKey, ClientConfig config, string stsToken)
            : this(credentials: new BasicServiceCredentials(accessKey: accessKeyId, secretKey: secretAccessKey, stsToken: stsToken), config: config)
        {
        }

        protected virtual void Initialize()
        {
        }

        #endregion

        #region Invoke methods

        internal TResponse Invoke<TRequest, TResponse>
        (
            TRequest                                 request,
            IMarshaller<IRequest, WebServiceRequest> marshaller,
            ResponseUnmarshaller                     unmarshaller
        )
            where TRequest : WebServiceRequest
            where TResponse : WebServiceResponse
        {
            ThrowIfDisposed();

            var executionContext = new ExecutionContext(
                                                        requestContext: new RequestContext
                                                        {
                                                            ClientConfig    = Config,
                                                            Marshaller      = marshaller,
                                                            OriginalRequest = request,
                                                            Signer          = Signer,
                                                            Unmarshaller    = unmarshaller,
                                                            IsAsync         = false
                                                        },
                                                        responseContext: new ResponseContext()
                                                       );

            var response = (TResponse)RuntimePipeline.InvokeSync(executionContext: executionContext).Response;
            return response;
        }

        internal IAsyncResult BeginInvoke<TRequest>
        (
            TRequest                                 request,
            IMarshaller<IRequest, WebServiceRequest> marshaller,
            ResponseUnmarshaller                     unmarshaller,
            AsyncCallback                            callback,
            object                                   state
        )
            where TRequest : WebServiceRequest
        {
            ThrowIfDisposed();

            var executionContext = new AsyncExecutionContext(
                                                             requestContext: new AsyncRequestContext
                                                             {
                                                                 ClientConfig    = Config,
                                                                 Marshaller      = marshaller,
                                                                 OriginalRequest = request,
                                                                 Signer          = Signer,
                                                                 Unmarshaller    = unmarshaller,
                                                                 Callback        = callback,
                                                                 State           = state,
                                                                 IsAsync         = true
                                                             },
                                                             responseContext: new AsyncResponseContext()
                                                            );

            var asyncResult = RuntimePipeline.InvokeAsync(executionContext: executionContext);
            return asyncResult;
        }

        internal static TResponse EndInvoke<TResponse>(IAsyncResult result)
            where TResponse : WebServiceResponse
        {
            if (result == null) throw new ArgumentNullException(paramName: "result", message: "Parameter result cannot be null.");

            var asyncResult = result as RuntimeAsyncResult;

            if (asyncResult == null) throw new ArgumentOutOfRangeException(paramName: "result", message: "Parameter result is not of type RuntimeAsyncResult.");

            using (asyncResult)
            {
                if (!asyncResult.IsCompleted) asyncResult.AsyncWaitHandle.WaitOne();

                if (asyncResult.Exception != null)
                {
                    AliyunSDKUtils.PreserveStackTrace(exception: asyncResult.Exception);
                    throw asyncResult.Exception;
                }

                return (TResponse)asyncResult.Response;
            }
        }

        #endregion

        #region Dispose methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (RuntimePipeline != null) RuntimePipeline.Dispose();

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(objectName: GetType().FullName);
        }

        #endregion
    }
}