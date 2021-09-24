using System.Net;
using System.Reflection;
using FS.Configuration.Startup;
using FS.DI;

namespace FS.Modules
{
    /// <summary>
    ///     系统核心模块（初始化时，最先执行的模块）
    /// </summary>
    public sealed class FarseerKernelModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(registrar: new BasicConventionalRegistrarInstaller());
            ServicePointManager.DefaultConnectionLimit = 512;
            ServicePointManager.UseNagleAlgorithm      = false;
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            foreach (var replaceAction in ((FarseerStartupConfiguration)Configuration).ServiceReplaceActions.Values) replaceAction();
            IocManager.RegisterAssemblyByConvention(assembly: GetType().GetTypeInfo().Assembly, config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }

        public override void PostInitialize()
        {
        }
    }
}