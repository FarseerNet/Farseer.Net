using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FS.Core;
using FS.DI;
using FS.Job.Configuration;
using FS.Job.Entity;
using FS.Modules;
using FS.Utils.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    public class JobModule : FarseerModule
    {
        public static ClientVO Client { get; set; }

        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            var fssAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<FssAttribute>();
            if (fssAttribute is
            {
                Enable: true
            })
            {
                var jobItemConfig = IocManager.Resolve<IConfigurationRoot>().GetSection(key: "FSS").Get<JobItemConfig>();
                if (jobItemConfig == null)
                {
                    IocManager.Logger<JobModule>().LogWarning(message: "未找到FSS配置，无法启动任务");
                    return;
                }

                Client = new ClientVO
                {
                    ClientIp   = IpHelper.GetIp,
                    Id         = SnowflakeId.GenerateId(),
                    ClientName = Environment.MachineName,
                    Jobs       = JobInstaller.JobImpList.Keys.Select(selector: o => o).ToArray()
                };

                FarseerApplication.AddInitCallback(() =>
                {
                    // 查找启用了Debug状态的job，立即执行
                    foreach (var jobType in JobInstaller.JobImpList)
                    {
                        var fssJobAttribute = jobType.Value.GetCustomAttribute<FssJobAttribute>();
                        if (fssJobAttribute == null || !fssJobAttribute.Debug) continue;
                        IocManager.Logger<JobModule>().LogDebug(message: $"Debug：启动{jobType.Key}。");
                        var sw = Stopwatch.StartNew();
                        try
                        {
                            Task.WaitAll(IocManager.Resolve<IFssJob>(name: $"fss_job_{jobType.Key}").Execute(context: new ReceiveContext(ioc: IocManager, jobType.Key, sw: sw, Jsons.ToObject<Dictionary<string, string>>(fssJobAttribute.DebugMetaData))));
                        }
                        catch (Exception e)
                        {
                            IocManager.Logger<JobModule>().LogError(exception: e, message: e.Message);
                        }
                        finally
                        {
                            IocManager.Logger<JobModule>().LogDebug(message: $"Debug：{jobType.Key} 耗时 {sw.ElapsedMilliseconds} ms");
                        }
                    }
                    
                    TaskQueueList.PullJob();
                    TaskQueueList.RunJob();
                });
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new JobInstaller(iocResolver: IocManager));
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
            AppContext.SetSwitch(switchName: "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);
        }
    }
}