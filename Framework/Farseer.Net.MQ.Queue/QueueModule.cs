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
            // 耗时：357 ms
            //模块初始化，实现IOC信息的注册
            IocManager.Container.Install(new QueueInstaller(_typeFinder));
        }
    }
}