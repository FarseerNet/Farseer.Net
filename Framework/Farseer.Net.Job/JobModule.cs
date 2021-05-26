using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using FS.DI;
using FS.Extends;
using FS.Job.Configuration;
using FS.Job.Entity;
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
        private static Dictionary<string, AsyncDuplexStreamingCall<ChannelRequest, CommandResponse>> dic = new();

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

                // 当前客户端支持的job
                var jobs = JobInstaller.JobImpList.Keys.Select(o => o).ToArray();
                // 开启本地调试状态
                if (jobItemConfig.Debug)
                {
                    IocManager.Logger<JobModule>().LogInformation($"开启Debug模式");
                    string[] debugJobs = jobItemConfig.DebugJobs.ToLower() == "all" ? jobs : jobItemConfig.DebugJobs.Split(',');
                    foreach (var debugJob in debugJobs)
                    {
                        IocManager.Logger<JobModule>().LogInformation($"Debug：启动{debugJob}。");
                        
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            IocManager.Resolve<IFssJob>($"fss_job_{debugJob}").Execute(new ReceiveContext(IocManager, sw));
                        }
                        catch (Exception e)
                        {
                            IocManager.Logger<JobModule>().LogError(e,e.Message);
                        }
                        finally
                        {
                            IocManager.Logger<JobModule>().LogInformation($"Debug：{debugJob} 耗时 {sw.ElapsedMilliseconds} ms");
                        }
                    }
                    return;
                }


                // 注册到服务端
                foreach (var server in jobItemConfig.Server.Split(','))
                {
                    foreach (var jobName in jobs)
                    {
                        var key = $"{server}|{jobName}";
                        dic[key] =IocManager.Resolve<ChannelClient>().Channel(server, jobName);
                    }
                }

                ThreadPool.QueueUserWorkItem(async state =>
                {
                    while (true)
                    {
                        Thread.Sleep(5000);

                        foreach (var rpc in dic)
                        {
                            var call = rpc.Value;
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
                            catch (Exception)
                            {
                                IocManager.Logger<JobModule>().LogDebug($"{rpc.Key}，未连接，尝试重新注册...");

                                var keys = rpc.Key.Split('|');
                                dic[rpc.Key] = IocManager.Resolve<ChannelClient>().Channel(keys[0], keys[1]);
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