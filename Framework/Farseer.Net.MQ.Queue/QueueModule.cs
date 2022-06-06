using System.Reflection;
using FS.DI;
using FS.Modules;
using FS.Reflection;

namespace FS.MQ.Queue
{
    /// <summary>
    ///     Rabbit模块
    /// </summary>
    public class QueueModule : FarseerModule
    {
        public QueueModule(ITypeFinder typeFinder)
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
            IocManager.Container.Install(new QueueInstaller(_typeFinder));
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
    }
}