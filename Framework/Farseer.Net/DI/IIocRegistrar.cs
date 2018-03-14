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
        /// <param name="registrar"></param>
        void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar);

		/// <summary>
		///     根据约定注册程序集
		/// </summary>
		/// <param name="type"></param>
		void RegisterAssemblyByConvention(Type type);

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config);

        /// <summary>
        ///     注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        void Register<T>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class;

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        void Register(Type type, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        ///     注册
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        void Register<TType, TImpl>(string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType;

        /// <summary>
        ///     注册实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="impl"></param>
        /// <param name="name"></param>
        /// <param name="lifeStyle"></param>
        void Register(Type type, Type impl, string name = "", DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsRegistered(Type type);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        bool IsRegistered<TType>();
    }
}