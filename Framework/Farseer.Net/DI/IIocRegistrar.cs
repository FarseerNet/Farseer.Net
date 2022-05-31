using System;
using System.Reflection;

namespace FS.DI
{
    /// <summary>
    ///     依赖注册接口
    /// </summary>
    public interface IIocRegistrar
    {
        /// <summary>
        ///     添加约定注册器
        /// </summary>
        void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar);

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        void RegisterAssemblyByConvention(Type type);

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config);

        /// <summary>
        ///     注册
        /// </summary>
        void Register<T>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class;

        /// <summary>
        ///     注册
        /// </summary>
        void Register(Type type, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        ///     注册
        /// </summary>
        void Register<TType, TImpl>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType;

        /// <summary>
        ///     注册实例
        /// </summary>
        void Register(Type type, Type impl, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        ///     是否注册
        /// </summary>
        bool IsRegistered(Type type);

        /// <summary>
        ///     是否注册
        /// </summary>
        bool IsRegistered<TType>();
    }
}