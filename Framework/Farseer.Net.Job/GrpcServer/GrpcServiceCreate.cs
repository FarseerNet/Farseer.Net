using System.Net;
using FS.DI;
using FS.Job.Configuration;
using FSS.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FS.Job.GrpcServer
{
    /// <summary>
    /// 启动一个GRPC服务
    /// </summary>
    public class GrpcServiceCreate
    {
        private readonly IIocManager _ioc;

        public GrpcServiceCreate(IIocManager ioc)
        {
            _ioc = ioc;
        }
        public void Start(JobItemConfig jobItemConfig)
        {
            Server server = new Server
            {
                Services =
                {
                    ReceiveNotify.BindService(new ReceiveNotifyService(_ioc))
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