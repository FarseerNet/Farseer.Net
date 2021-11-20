using System;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace Farseer.Net.Grpc
{
    /// <summary>
    ///     链路追踪（Grpc入口）
    /// </summary>
    public class LinkTrackInterceptor : Interceptor
    {
        /// <summary>
        ///     GRPC服务端，接收请求
        /// </summary>
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var contextId = context.RequestHeaders.FirstOrDefault(predicate: o => o.Key == "fscontextid")?.Value;
            if (!string.IsNullOrWhiteSpace(value: contextId))
            {
                var parentAppId = context.RequestHeaders.FirstOrDefault(predicate: o => o.Key == "fsappid")?.Value;
                FsLinkTrack.Current.Set(contextId: contextId, parentAppId: parentAppId);
            }

            TResponse result    = null;
            var       dicHeader = context.RequestHeaders.ToDictionary(keySelector: o => o.Key, elementSelector: o => o.Value);
            var       path      = $"http://{context.Host}{context.Method.ToLower()}";

            using (var trackEnd = FsLinkTrack.TrackApiServer(domain: context.Host, path: path, method: "GRPC", contentType: "application/grpc", headerDictionary: dicHeader, requestBody: JsonConvert.SerializeObject(value: request), ip: context.Peer))
            {
                try
                {
                    result = await continuation(request: request, context: context);
                    trackEnd.SetDownstreamResponseBody(responseBody: JsonConvert.SerializeObject(value: result));
                }
                catch (Exception e)
                {
                    trackEnd.Exception(exception: e);
                    throw;
                }
            }

            // 写入链路追踪
            IocManager.GetService<ILinkTrackQueue>().Enqueue();
            return result;
        }

        /// <summary>
        ///     客户端请求GRPC服务时，要添加的头部信息
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            using (FsLinkTrack.TrackGrpc(server: context.Method.ServiceName, action: context.Method.Name))
            {
                var result = continuation(request: request, context: Continuation(context: context));
                result.GetAwaiter().GetResult();
                return result;
            }
        }

        /// <summary>
        ///     客户端请求GRPC服务时，要添加的头部信息
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            using (FsLinkTrack.TrackGrpc(server: context.Method.ServiceName, action: context.Method.Name))
            {
                return continuation(request: request, context: Continuation(context: context));
            }
        }

        /// <summary>
        ///     继承后，可自定义添加头部或其它信息
        /// </summary>
        protected virtual CallOptions SetCallOptions(CallOptions callOptions) => callOptions;

        /// <summary>
        ///     添加通用头部信息
        /// </summary>
        private ClientInterceptorContext<TRequest, TResponse> Continuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var callOptions = context.Options.WithHeaders(headers: new Metadata
            {
                { "FsContextId", FsLinkTrack.Current.Get().ContextId },
                { "FsAppId", FsLinkTrack.Current.Get().AppId }
            });

            callOptions = SetCallOptions(callOptions: callOptions);
            // 添加头部信息
            return new ClientInterceptorContext<TRequest, TResponse>(method: context.Method, host: context.Host, options: callOptions);
        }
    }
}