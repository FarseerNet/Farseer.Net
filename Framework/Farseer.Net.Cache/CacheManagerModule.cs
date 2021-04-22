using System.Collections.Generic;
using System.Reflection;
using FS.Cache.Configuration;
using FS.Configuration;
using FS.DI;
using FS.Modules;

namespace FS.Cache
{
    /// <summary>
    ///     Redis模块
    /// </summary>
    public class CacheManagerModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
            // 如果Redis配置没有创建，则创建它
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new CacheManagerInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}