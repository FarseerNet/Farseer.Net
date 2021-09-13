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
            if (fssAttribute is { Enable: true })
            {
                var jobItemConfig = IocManager.Resolve<IConfigurationRoot>().GetSection("FSS").Get<JobItemConfig>();
                if (jobItemConfig == null)
                {
                    IocManager.Logger<JobModule>().LogWarning($"未找到FSS配置，无法启动任务");
                    return;
                }

                Client = new ClientVO
                {
                    ClientIp   = IpHelper.GetIp,
                    Id         = SnowflakeId.GenerateId(),
                    ClientName = Environment.MachineName,
                    Jobs       = JobInstaller.JobImpList.Keys.Select(o => o).ToArray()
                };

                // 开启本地调试状态
                if (jobItemConfig.Debug)
                {
                    // 待系统初始化完后执行
                    FarseerApplication.AddInitCallback(() =>
                    {
                        IocManager.Logger<JobModule>().LogInformation($"开启Debug模式");
                        string[] debugJobs = jobItemConfig.DebugJobs.ToLower() == "all" ? Client.Jobs : jobItemConfig.DebugJobs.Split(',');
                        foreach (var debugJob in debugJobs)
                        {
                            IocManager.Logger<JobModule>().LogInformation($"Debug：启动{debugJob}。");

                            var sw = Stopwatch.StartNew();
                            try
                            {
                                IocManager.Resolve<IFssJob>($"fss_job_{debugJob}").Execute(new ReceiveContext(IocManager, sw, jobItemConfig.DebugMetaData));
                            }
                            catch (Exception e)
                            {
                                IocManager.Logger<JobModule>().LogError(e, e.Message);
                            }
                            finally
                            {
                                IocManager.Logger<JobModule>().LogInformation($"Debug：{debugJob} 耗时 {sw.ElapsedMilliseconds} ms");
                            }
                        }
                    });
                }
                else
                {
                    TaskQueueList.RunJob();
                }
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new JobInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }
    }
}