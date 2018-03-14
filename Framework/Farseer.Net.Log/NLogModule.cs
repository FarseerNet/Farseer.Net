using System.Reflection;
using FS.Configuration;
using FS.DI;
using FS.Log.Configuration;
using FS.Modules;

namespace FS.Log
{
    public class NLogModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            var configResolver = IocManager.Resolve<IConfigResolver>();
            InitConfig(configResolver);
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new NLogInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }

        private static void InitConfig(IConfigResolver configResolver)
        {
            var config = configResolver.NLogConfig();
            if (config == null || config.AppName == null)
            {
                configResolver.Set(new NLogConfig {AppName = "Default"} );
                configResolver.Save();
            }
        }
    }
}
