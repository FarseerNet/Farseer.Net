using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

                // 开启本地调试状态
                if (jobItemConfig.Debug)
                {
                    // 待系统初始化完后执行
                    FarseerApplication.AddInitCallback(act: () =>
                    {
                        IocManager.Logger<JobModule>().LogInformation(message: "开启Debug模式");
                        var debugJobs = jobItemConfig.DebugJobs.ToLower() == "all" ? Client.Jobs : jobItemConfig.DebugJobs.Split(',');
                        foreach (var debugJob in debugJobs)
                        {
                            IocManager.Logger<JobModule>().LogInformation(message: $"Debug：启动{debugJob}。");

                            var sw = Stopwatch.StartNew();
                            try
                            {
                                IocManager.Resolve<IFssJob>(name: $"fss_job_{debugJob}").Execute(context: new ReceiveContext(ioc: IocManager, sw: sw, debugMetaData: jobItemConfig.DebugMetaData));
                            }
                            catch (Exception e)
                            {
                                IocManager.Logger<JobModule>().LogError(exception: e, message: e.Message);
                            }
                            finally
                            {
                                IocManager.Logger<JobModule>().LogInformation(message: $"Debug：{debugJob} 耗时 {sw.ElapsedMilliseconds} ms");
                            }
                        }
                    });
                }
                else
                    TaskQueueList.RunJob();
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