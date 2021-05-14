using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using FS.DI;
using FS.Extends;
using FS.Job.Configuration;
using FS.Job.GrpcClient;
using FS.Modules;
using FSS.GrpcService;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    public class JobModule : FarseerModule
    {
        private static Dictionary<string, List<AsyncDuplexStreamingCall<ChannelRequest, CommandResponse>>> dic = new();

        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            var fssAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<FssAttribute>();
            if (fssAttribute is {Enable: true})
            {
                var jobItemConfig = IocManager.Resolve<IConfigurationRoot>().GetSection("FSS").Get<JobItemConfig>();
                if (jobItemConfig == null)
                {
                    IocManager.Logger<JobModule>().LogWarning($"未找到FSS配置，无法注册到FSS平台");
                    return;
                }

                // 注册到服务端
                var jobs = JobInstaller.JobImpList.Keys.Select(o => o).ToArray();
                foreach (var server in jobItemConfig.Server.Split(','))
                {
                    dic[server] = new();
                    var arrJob = string.Join(",", jobs);
                    for (int i = 0; i < Environment.ProcessorCount; i++)
                    {
                        dic[server].Add(IocManager.Resolve<ChannelClient>().Channel(server, arrJob));
                    }
                }

                ThreadPool.QueueUserWorkItem(async state =>
                {
                    while (true)
                    {
                        Thread.Sleep(5000);
                        
                        foreach (var rpc in dic)
                        {
                            for (var index = 0; index < rpc.Value.Count; index++)
                            {
                                var call = rpc.Value[index];
                                try
                                {
                                    // 尝试发送心跳，看连接是否正常
                                    await call.RequestStream.WriteAsync(new ChannelRequest
                                    {
                                        Command   = "Heartbeat",
                                        RequestAt = DateTime.Now.ToTimestamps(),
                                        Data      = ""
                                    });

                                    IocManager.Logger<JobModule>().LogDebug($"发送心跳===> {rpc.Key} 心跳");
                                }
                                catch (Exception e)
                                {
                                    IocManager.Logger<JobModule>().LogDebug($"{rpc.Key}，未连接，尝试重新注册...");

                                    var arrJob = string.Join(",", jobs);
                                    dic[rpc.Key][index] = IocManager.Resolve<ChannelClient>().Channel(rpc.Key, arrJob);
                                }
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new JobInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }
    }
}