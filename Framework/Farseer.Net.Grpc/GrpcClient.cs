using System;
using Farseer.Net.Grpc.Configuration;
using FS.DI;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Farseer.Net.Grpc
{
    /// <summary>
    /// CSL GRPC服务
    /// </summary>
    public class GrpcClient : IGrpcClient
    {
        private readonly GrpcItemConfig _config;
        private          GrpcChannel    _grpcChannel;
        private          CallInvoker    _callInvoker;
        public           IIocManager    IocManager { get; set; }

        public GrpcClient(GrpcItemConfig config)
        {
            _config = config;
            Connect();
        }

        private void Connect()
        {
            _grpcChannel?.Dispose();
            _grpcChannel = GrpcChannel.ForAddress(_config.Server);
            _callInvoker = _grpcChannel.Intercept(new GrpcInterceptor());
        }

        /// <summary>
        /// 访问Grpc服务
        /// </summary>
        /// <param name="func"></param>
        /// <typeparam name="TClientBase"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public TResponse Try<TClientBase, TResponse>(Func<TClientBase, TResponse> func) where TClientBase : ClientBase<TClientBase>
        {
            try
            {
                var clientBase = (TClientBase) Activator.CreateInstance(typeof(TClientBase), _callInvoker);
                return func(clientBase);
            }
            catch (RpcException e)
            {
                return Catch<TResponse>(e, nameof(TClientBase));
            }
            catch (Exception e)
            {
                IocManager.Logger<GrpcClient>().LogError(e.Message, e);
                throw;
            }
        }

        private TResponse Catch<TResponse>(RpcException e, string clientName)
        {
            switch (e.Status.StatusCode)
            {
                case StatusCode.Internal when e.Status.Detail.Contains("timed out"):
                    IocManager.Logger<GrpcClient>().LogWarning($"请求{clientName}超时，自动重连");
                    Connect();
                    throw new Exception($"请求{clientName}超时，请重试");
                case StatusCode.DeadlineExceeded:
                    IocManager.Logger<GrpcClient>().LogWarning($"请求{clientName}超时 {e.ToString()}");
                    throw new Exception($"请求{clientName}超时");
                case StatusCode.Unimplemented:
                    IocManager.Logger<GrpcClient>().LogWarning($"请求{clientName}的服务不存在");
                    throw new Exception($"请求{clientName}的服务不存在");
                default:
                    IocManager.Logger<GrpcClient>().LogError(e, e.Message);
                    throw e;
            }
        }
    }
}