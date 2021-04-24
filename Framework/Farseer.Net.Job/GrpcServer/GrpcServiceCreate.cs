using System.Net;
using FS.DI;
using FS.Job.Configuration;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FS.Job.GrpcServer
{
    /// <summary>
    /// 启动一个GRPC服务
    /// </summary>
    public class GrpcServiceCreate
    {
        public void Start(JobItemConfig jobItemConfig)
        {
            Server server = new Server
            {
                Services =
                {
                    //CallJob.BindService(new ReceiveNotifyService())
                },
                Ports    =
                {
                    new ServerPort(IPAddress.Any.ToString(), jobItemConfig.GrpcServicePort, ServerCredentials.Insecure)
                },
                
            };
            server.Start();
            IocManager.Instance.Logger<JobModule>().LogInformation($"启动GRPC服务：http://{IPAddress.Any.ToString()}:{jobItemConfig.GrpcServicePort}");
        }
    }
}