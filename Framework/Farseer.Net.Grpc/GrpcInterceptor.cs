using System;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Farseer.Net.Grpc
{
    /// <summary>
    /// 调用CSC RPC时拦截
    /// </summary>
    public class GrpcInterceptor : Interceptor
    {
        private int _rpcRequestTimeout = 2000;

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(request, Continuation(context));
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(request, Continuation(context));
        }

        /// <summary>
        /// 添加通用头部信息
        /// </summary>
        private ClientInterceptorContext<TRequest, TResponse> Continuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var callOpt                        = context.Options;
            if (_rpcRequestTimeout > 0) callOpt = callOpt.WithDeadline(DateTime.UtcNow.AddMilliseconds(_rpcRequestTimeout));

            return new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOpt);
        }
    }
}