using Farseer.Net.Grpc;
using FS.Core;
using FS.Core.Net;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class Extends
    {
        /// <summary>
        ///     类型转换
        /// </summary>
        public static ApiResponseJson ToApi<TData>(this RpcResponse rpcResponse)
        {
            if (!rpcResponse.Status) return ApiResponseJson.Error(statusMessage: rpcResponse.StatusMessage, statusCode: rpcResponse.StatusCode);
            return ApiResponseJson.Success(statusMessage: rpcResponse.StatusMessage, data: Jsons.ToObject<TData>(obj: rpcResponse.Data));
        }

        /// <summary>
        ///     类型转换
        /// </summary>
        public static TData ToEntity<TData>(this RpcResponse rpcResponse) => Jsons.ToObject<TData>(obj: rpcResponse.Data);
    }
}