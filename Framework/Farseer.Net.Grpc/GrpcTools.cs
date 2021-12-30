using System;
using System.Reflection;
using System.Threading.Tasks;
using Farseer.Net.Grpc;
using FS.Core.Exception;
using FS.DI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FS.Grpc
{
    public class GrpcTools
    {
        /// <summary>
        ///     增加异常处理
        /// </summary>
        public static Task<RpcResponse> Try<TResult>(Func<TResult> func, string successMessage = "成功")
        {
            try
            {
                return Task.FromResult(result: new RpcResponse { Status = true, StatusCode = 200, StatusMessage = successMessage, Data = JsonConvert.SerializeObject(value: func()) });
            }
            catch (RefuseException e)
            {
                return Task.FromResult(result: new RpcResponse { Status = false, StatusCode = 403, StatusMessage = e.Message, Data = e.Message });
            }
            catch (TargetInvocationException e)
            {
                switch (e.InnerException)
                {
                    case RefuseException _: return Task.FromResult(result: new RpcResponse { Status = false, StatusCode = 403, StatusMessage = e.InnerException.Message, Data = e.InnerException.Message });
                    default:
                        IocManager.Instance.Logger<GrpcTools>().LogError(exception: e.InnerException, message: e.InnerException.ToString());
                        return Task.FromResult(result: new RpcResponse { Status = false, StatusCode = 500, StatusMessage = e.InnerException.Message, Data = e.InnerException.Message });
                }
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<GrpcTools>().LogError(exception: e, message: e.ToString());
                return Task.FromResult(result: new RpcResponse { Status = false, StatusCode = 500, StatusMessage = e.Message, Data = e.Message });
            }
        }
    }
}