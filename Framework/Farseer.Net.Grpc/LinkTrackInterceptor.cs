using System;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.Extends;
using FS.LinkTrack;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace Farseer.Net.Grpc
{
    /// <summary>
    /// 链路追踪（Grpc入口）
    /// </summary>
    public class LinkTrackInterceptor : Interceptor
    {
        /// <summary>
        /// GRPC服务端，接收请求
        /// </summary>
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var contextId = (context.RequestHeaders.FirstOrDefault(o => o.Key == "fscontextid")?.Value);
            if (!string.IsNullOrWhiteSpace(contextId))
            {
                var parentAppId = (context.RequestHeaders.FirstOrDefault(o => o.Key == "fsappid")?.Value);
                FsLinkTrack.Current.Set(contextId, parentAppId);
            }

            TResponse result = null;
            var       dicHeader = context.RequestHeaders.ToDictionary(o => o.Key, o => o.Value);
            var       path      = $"http://{context.Host}{context.Method.ToLower()}";
            
            using (var trackEnd = FsLinkTrack.TrackApiServer(context.Host, path, "GRPC", "application/grpc", dicHeader, JsonConvert.SerializeObject(request), context.Peer))
            {
                try
                {
                    result = await continuation(request, context);
                    trackEnd.SetDownstreamResponseBody(JsonConvert.SerializeObject(result));
                }
                catch (Exception e)
                {
                    trackEnd.Exception(e);
                    throw;
                }
            }

            // 写入链路追踪
            IocManager.Instance.Resolve<ILinkTrackQueue>().Enqueue();
            return result;
        }

        /// <summary>
        /// 客户端请求GRPC服务时，要添加的头部信息
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            using (FsLinkTrack.TrackGrpc(context.Method.ServiceName, context.Method.Name))
            {
                var result = continuation(request, Continuation(context));
                result.GetAwaiter().GetResult();
                return result;
            }
        }

        /// <summary>
        /// 客户端请求GRPC服务时，要添加的头部信息
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            using (FsLinkTrack.TrackGrpc(context.Method.ServiceName, context.Method.Name))
            {
                return continuation(request, Continuation(context));
            }
        }

        /// <summary>
        /// 继承后，可自定义添加头部或其它信息
        /// </summary>
        protected virtual CallOptions SetCallOptions(CallOptions callOptions) => callOptions;

        /// <summary>
        /// 添加通用头部信息
        /// </summary>
        private ClientInterceptorContext<TRequest, TResponse> Continuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var callOptions = context.Options.WithHeaders(new Metadata()
            {
                {"FsContextId", FsLinkTrack.Current.Get().ContextId},
                {"FsAppId", FsLinkTrack.Current.Get().AppId}
            });

            callOptions = SetCallOptions(callOptions);
            // 添加头部信息
            return new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOptions);
        }
    }
}