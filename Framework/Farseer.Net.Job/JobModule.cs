using System.Linq;
using System.Reflection;
using FS.DI;
using FS.Job.Configuration;
using FS.Job.GrpcClient;
using FS.Modules;
using Grpc.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    public class JobModule : FarseerModule
    {
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
                    IocManager.Resolve<ChannelClient>().Channel(server, jobs);
                }
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new JobInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
        }
    }
}