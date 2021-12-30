using System.Reflection;
using FS.DI;
using FS.Modules;
using FS.Reflection;

namespace FS.MQ.Rabbit
{
    /// <summary>
    ///     Rabbit模块
    /// </summary>
    public class RabbitModule : FarseerModule
    {
        public RabbitModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        private readonly ITypeFinder _typeFinder;

        /// <inheritdoc />
        public override void PreInitialize()
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new RabbitInstaller(_typeFinder));
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}