using System.Reflection;
using FS.DI;
using FS.Job.Configuration;
using FS.Modules;
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
            var jobItemConfig = DI.IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("Job").Get<JobItemConfig>();
            
            // 启动RPC服务
            new GrpcServiceCreate().Start(jobItemConfig.GrpcServicePort);
            
            // 注册到服务端
            new ServiceRegister().Register(jobItemConfig.Server);
            
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