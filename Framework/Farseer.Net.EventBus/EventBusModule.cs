using System.Reflection;
using FS.DI;
using FS.Modules;

namespace FS.EventBus
{
    /// <summary>
    ///     事件总线模块
    /// </summary>
    public class EventBusModule : FarseerModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new EventBusInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}