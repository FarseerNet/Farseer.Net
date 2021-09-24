using System.Reflection;

namespace FS.DI
{
    /// <summary>
    ///     约定注册上下文
    /// </summary>
    internal class ConventionalRegistrationContext : IConventionalRegistrationContext
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="assembly"> </param>
        /// <param name="iocManager"> </param>
        /// <param name="config"> </param>
        internal ConventionalRegistrationContext(Assembly assembly, IIocManager iocManager, ConventionalRegistrationConfig config)
        {
            Assembly   = assembly;
            IocManager = iocManager;
            Config     = config;
        }

        /// <summary>
        ///     要注册的程序集
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        ///     依赖注入管理器
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     约定注册配置
        /// </summary>
        public ConventionalRegistrationConfig Config { get; }
    }
}