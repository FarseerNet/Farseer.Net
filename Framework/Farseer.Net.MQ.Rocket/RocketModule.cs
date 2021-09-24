using System.Reflection;
using FS.DI;
using FS.Modules;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     RocketMQ模块
    /// </summary>
    public class RocketModule : FarseerModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new RocketInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}