using System.Threading.Tasks;
using FSS.GrpcService;
using Grpc.Core;

namespace FS.Job.Abstract
{
    /// <summary>
    /// 根据不同的命令，提供不同的处理
    /// </summary>
    public interface IRemoteCommand
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="client">连接FSS平台的客户端 </param>
        /// <param name="requestStream">请求流</param>
        /// <param name="responseStream">响应流</param>
        Task InvokeAsync(FssServer.FssServerClient client, IClientStreamWriter<ChannelRequest> requestStream, IAsyncStreamReader<CommandResponse> responseStream);

        /// <summary>
        /// 处理
        /// </summary>
        void Invoke(string data);
    }
}