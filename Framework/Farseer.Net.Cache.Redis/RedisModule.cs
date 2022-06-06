using System.Reflection;
using FS.DI;
using FS.Modules;

namespace FS.Cache.Redis
{
    /// <summary>
    ///     Redis模块
    /// </summary>
    [DependsOn(typeof(CacheModule))]
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
            IocManager.Container.Install(new RedisInstaller(iocResolver: IocManager));
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}