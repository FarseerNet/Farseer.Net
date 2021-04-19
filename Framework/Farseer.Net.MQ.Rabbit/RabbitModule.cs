using System.Reflection;
using FS.DI;
using FS.Modules;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     RocketMQ模块
    /// </summary>
    public class RabbitModule : FarseerModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new RabbitInstaller());
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
        }
    }
}