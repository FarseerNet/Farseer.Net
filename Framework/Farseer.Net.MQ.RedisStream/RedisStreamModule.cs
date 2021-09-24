using System.Reflection;
using FS.Cache.Redis;
using FS.DI;
using FS.Modules;

namespace FS.MQ.RedisStream
{
    /// <summary>
    ///     RocketMQ模块
    /// </summary>
    [DependsOn(typeof(RedisModule))]
    public class RedisStreamModule : FarseerModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new RedisStreamInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}