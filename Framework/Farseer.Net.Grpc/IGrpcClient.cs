using System;
using Grpc.Core;

namespace FS.Grpc
{
    public interface IGrpcClient
    {
        /// <summary>
        ///     访问Grpc服务
        /// </summary>
        /// <param name="func"> </param>
        /// <typeparam name="TClientBase"> </typeparam>
        /// <typeparam name="TResponse"> </typeparam>
        /// <returns> </returns>
        TResponse Try<TClientBase, TResponse>(Func<TClientBase, TResponse> func) where TClientBase : ClientBase<TClientBase>;
    }
}