
using System.Reflection;

namespace FS.DI
{
    /// <summary>
    ///     约定注册上下文接口
    /// </summary>
    public interface IConventionalRegistrationContext
    {
        /// <summary>
        ///     要注册的程序集
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        ///     依赖注入管理器
        /// </summary>
        IIocManager IocManager { get; }

        /// <summary>
        ///     约定注册配置
        /// </summary>
        ConventionalRegistrationConfig Config { get; }
    }
}