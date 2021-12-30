using System;
using Farseer.Net.Grpc;
using FS.DI;
using FS.Grpc.Configuration;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace FS.Grpc
{
    /// <summary>
    ///     CSL GRPC服务
    /// </summary>
    public class GrpcClient : IGrpcClient
    {
        private readonly GrpcItemConfig _config;
        private          CallInvoker    _callInvoker;
        private          GrpcChannel    _grpcChannel;

        public GrpcClient(GrpcItemConfig config)
        {
            _config = config;
            Connect();
        }

        public IIocManager IocManager { get; set; }

        /// <summary>
        ///     访问Grpc服务
        /// </summary>
        /// <param name="func"> </param>
        /// <typeparam name="TClientBase"> </typeparam>
        /// <typeparam name="TResponse"> </typeparam>
        /// <returns> </returns>
        public TResponse Try<TClientBase, TResponse>(Func<TClientBase, TResponse> func) where TClientBase : ClientBase<TClientBase>
        {
            try
            {
                var clientBase = (TClientBase)Activator.CreateInstance(type: typeof(TClientBase), _callInvoker);
                return func(arg: clientBase);
            }
            catch (RpcException e)
            {
                return Catch<TResponse>(e: e, clientName: nameof(TClientBase));
            }
            catch (Exception e)
            {
                IocManager.Logger<GrpcClient>().LogError(message: e.Message, e);
                throw;
            }
        }

        private void Connect()
        {
            _grpcChannel?.Dispose();
            _grpcChannel = GrpcChannel.ForAddress(address: _config.Server);
            _callInvoker = _grpcChannel.Intercept(interceptor: new GrpcInterceptor());
        }

        private TResponse Catch<TResponse>(RpcException e, string clientName)
        {
            switch (e.Status.StatusCode)
            {
                case StatusCode.Internal when e.Status.Detail.Contains(value: "timed out"):
                    IocManager.Logger<GrpcClient>().LogWarning(message: $"请求{clientName}超时，自动重连");
                    Connect();
                    throw new Exception(message: $"请求{clientName}超时，请重试");
                case StatusCode.DeadlineExceeded:
                    IocManager.Logger<GrpcClient>().LogWarning(message: $"请求{clientName}超时 {e}");
                    throw new Exception(message: $"请求{clientName}超时");
                case StatusCode.Unimplemented:
                    IocManager.Logger<GrpcClient>().LogWarning(message: $"请求{clientName}的服务不存在");
                    throw new Exception(message: $"请求{clientName}的服务不存在");
                default:
                    IocManager.Logger<GrpcClient>().LogError(exception: e, message: e.Message);
                    throw e;
            }
        }
    }
}