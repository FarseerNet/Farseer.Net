using System;
using FS.Core.LinkTrack;
using FS.Extends;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Farseer.Net.Grpc
{
    /// <summary>
    /// 注册公司ID拦截器
    /// </summary>
    public class LinkTrackInterceptor : Interceptor
    {
        /*
         使用中间件代替
        /// <summary>
        /// GRPC服务端，接收请求
        /// </summary>
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var contextId = (context.RequestHeaders.FirstOrDefault(o => o.Key == "fscontextid")?.Value);
            if (!string.IsNullOrWhiteSpace(contextId))
            {
                FsLinkTrack.Current.Set(contextId);
            }
            
            FsLinkTrack.Current.Set(new LinkTrackDetail
            {
                CallType          = EumCallType.GrpcServer,
                StartTs           = DateTime.Now.ToTimestamps(),
                GrpcLinkTrack = new GrpcLinkTrackDetail()
                {
                    Server = context.Method.Substring(0, context.Method.LastIndexOf('/')),
                    Action = context.Method.Split('/').LastOrDefault()
                }
            });
            var result =  await continuation(request, context);
            
            // 写入链路追踪
            LinkTrackQueue.Enqueue();
            return result;
        }
*/

        /// <summary>
        /// 客户端请求GRPC服务时，要添加的头部信息
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            FsLinkTrack.Current.Set(new LinkTrackDetail
            {
                CallType = EumCallType.GrpcClient,
                StartTs  = DateTime.Now.ToTimestamps(),
                GrpcLinkTrack = new GrpcLinkTrackDetail()
                {
                    Server = context.Method.ServiceName,
                    Action = context.Method.Name,
                }
            });
            return continuation(request, Continuation(context));
        }

        /// <summary>
        /// 客户端请求GRPC服务时，要添加的头部信息
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            FsLinkTrack.Current.Set(new LinkTrackDetail
            {
                CallType = EumCallType.GrpcClient,
                StartTs  = DateTime.Now.ToTimestamps(),
                GrpcLinkTrack = new GrpcLinkTrackDetail()
                {
                    Server = context.Method.ServiceName,
                    Action = context.Method.Name,
                }
            });
            return continuation(request, Continuation(context));
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
                {"FsContextId", FsLinkTrack.Current.Get().ContextId.ToString()}
            });

            callOptions = SetCallOptions(callOptions);
            // 添加头部信息
            return new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOptions);
        }
    }
}