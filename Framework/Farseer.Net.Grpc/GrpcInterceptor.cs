using System;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Farseer.Net.Grpc
{
    /// <summary>
    ///     调用CSC RPC时拦截
    /// </summary>
    public class GrpcInterceptor : Interceptor
    {
        private readonly int _rpcRequestTimeout = 2000;

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation) => continuation(request: request, context: Continuation(context: context));

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation) => continuation(request: request, context: Continuation(context: context));

        /// <summary>
        ///     添加通用头部信息
        /// </summary>
        private ClientInterceptorContext<TRequest, TResponse> Continuation<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var callOpt                         = context.Options;
            if (_rpcRequestTimeout > 0) callOpt = callOpt.WithDeadline(deadline: DateTime.UtcNow.AddMilliseconds(value: _rpcRequestTimeout));

            return new ClientInterceptorContext<TRequest, TResponse>(method: context.Method, host: context.Host, options: callOpt);
        }
    }
}