using System.Threading.Tasks;
using FS.DI;
using FS.Job.Abstract;
using FSS.GrpcService;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FS.Job.RemoteCall
{
    /// <summary>
    /// 忽略命令
    /// </summary>
    public class IgnoreCommand : IRemoteCommand
    {
        public IIocManager IocManager { get; set; }
        public Task InvokeAsync(FssServer.FssServerClient client, IClientStreamWriter<ChannelRequest> requestStream, IAsyncStreamReader<CommandResponse> responseStream)
        {
            Invoke(responseStream.Current.Data);
            return Task.FromResult(0);
        }
        
        /// <summary>
        /// 处理
        /// </summary>
        public void Invoke(string data)
        {
            IocManager.Logger<PrintCommand>().LogInformation(data);
        }
    }
}