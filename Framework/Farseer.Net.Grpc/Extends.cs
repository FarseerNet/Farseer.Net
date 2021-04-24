using Farseer.Net.Grpc;
using FS.Core;
using FS.Core.Net;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class Extends
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        public static ApiResponseJson ToApi<TData>(this RpcResponse rpcResponse)
        {
            if (!rpcResponse.Status) return ApiResponseJson.Error(rpcResponse.StatusMessage, rpcResponse.StatusCode);
            return ApiResponseJson.Success(rpcResponse.StatusMessage, Jsons.ToObject<TData>(rpcResponse.Data));
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        public static TData ToEntity<TData>(this RpcResponse rpcResponse)
        {
            return Jsons.ToObject<TData>(rpcResponse.Data);
        }
    }
}