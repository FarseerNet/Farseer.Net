using System;
using System.Threading.Tasks;
using FS.Core.Exception;
using FS.DI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Farseer.Net.Grpc
{
    public class GrpcTools
    {
        /// <summary>
        /// 增加异常处理
        /// </summary>
        public static Task<RpcResponse> Try<TResult>(Func<TResult> func, string successMessage = "成功")
        {
            try
            {
                return Task.FromResult(new RpcResponse {Status = true, StatusCode = 200, StatusMessage = successMessage, Data = JsonConvert.SerializeObject(func())});
            }
            catch (RefuseException e)
            {
                return Task.FromResult(new RpcResponse {Status = false, StatusCode = 403, StatusMessage = e.Message, Data = e.Message});
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                switch (e.InnerException)
                {
                    case RefuseException _:
                        return Task.FromResult(new RpcResponse {Status = false, StatusCode = 403, StatusMessage = e.InnerException.Message, Data = e.InnerException.Message});
                    default:
                        IocManager.Instance.Logger<GrpcTools>().LogError(e.InnerException, e.InnerException.ToString());
                        return Task.FromResult(new RpcResponse {Status = false, StatusCode = 500, StatusMessage = e.InnerException.Message, Data = e.InnerException.Message});
                }
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<GrpcTools>().LogError(e, e.ToString());
                return Task.FromResult(new RpcResponse {Status = false, StatusCode = 500, StatusMessage = e.Message, Data = e.Message});
            }
        }
    }
}