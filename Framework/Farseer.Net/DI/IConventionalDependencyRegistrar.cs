namespace Farseer.Net.DI
{
    /// <summary>
    ///     约定注册接口
    /// </summary>
    public interface IConventionalDependencyRegistrar
    {
        /// <summary>
        ///     注册程序集
        /// </summary>
        /// <param name="context">约定注册上下文</param>
        void RegisterAssembly(IConventionalRegistrationContext context);
    }
}