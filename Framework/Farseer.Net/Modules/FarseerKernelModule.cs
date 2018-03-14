using System.Net;
using System.Reflection;
using FS.Configuration.Startup;
using FS.DI;

namespace FS.Modules
{
    /// <summary>
    ///     系统核心模块
    /// </summary>
    public sealed class FarseerKernelModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            // 如果Redis配置没有创建，则创建它
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            //todo:SystemConfigBuilder.LoadConfig();
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            foreach (var replaceAction in ((FarseerStartupConfiguration)Configuration).ServiceReplaceActions.Values) { replaceAction(); }
#if CORE
			IocManager.RegisterAssemblyByConvention(typeof(FarseerKernelModule).GetTypeInfo().Assembly, new ConventionalRegistrationConfig { InstallInstallers = false });
#else
			IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
#endif
		}

        public override void PostInitialize()
        {
            ServicePointManager.DefaultConnectionLimit = 512;
        }
    }
}