using System.Net;
using Farseer.Net.Job;
using FS.DI;
using FS.Job.GrpcServer;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    /// <summary>
    /// 启动一个GRPC服务
    /// </summary>
    public class GrpcServiceCreate
    {
        public void Start(int port)
        {
            Server server = new Server
            {
                Services = {CallJob.BindService(new CallJobService())},
                Ports    = {new ServerPort(IPAddress.Any.ToString(), port, ServerCredentials.Insecure)}
            };
            server.Start();
            IocManager.Instance.Logger<JobModule>().LogInformation($"启动GRPC服务：http://{IPAddress.Any.ToString()}:{port}");
        }
    }
}