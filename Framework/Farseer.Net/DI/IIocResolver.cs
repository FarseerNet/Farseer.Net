using System;

namespace FS.DI
{
    /// <summary>
    ///     依赖注入依赖获取接口
    /// </summary>
    public interface IIocResolver
    {
        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Resolve<T>(string name = "");

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        T Resolve<T>(Type type, string name = "");

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argumentsAsAnonymousType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        T Resolve<T>(object argumentsAsAnonymousType, string name = "");

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object Resolve(Type type, string name = "");

        /// <summary>
        ///     获取实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentsAsAnonymousType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object Resolve(Type type, object argumentsAsAnonymousType, string name = "");

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argumentsAsAnonymousType"></param>
        /// <returns></returns>
        T[] ResolveAll<T>(object argumentsAsAnonymousType);

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object[] ResolveAll(Type type);

        /// <summary>
        ///     获取所有实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentsAsAnonymousType"></param>
        /// <returns></returns>
        object[] ResolveAll(Type type, object argumentsAsAnonymousType);

        /// <summary>
        ///     释放对象
        /// </summary>
        /// <param name="obj"></param>
        void Release(object obj);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsRegistered(Type type);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IsRegistered<T>();
    }
}