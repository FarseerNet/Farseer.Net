using System.Reflection;
using FS.Configuration;
using FS.DI;
using FS.Log.Configuration;
using FS.Modules;
using Microsoft.Extensions.Logging;

namespace FS.Log
{
    public class NLogModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new NLogInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}
