using System.Collections.Generic;
using System.Reflection;
using FS.Cache.Redis.Configuration;
using FS.Configuration;
using FS.DI;
using FS.Modules;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis模块
    /// </summary>
    [DependsOn(typeof(CacheManagerModule))]
    public class RedisModule : FarseerModule
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
            IocManager.Container.Install(new RedisInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}