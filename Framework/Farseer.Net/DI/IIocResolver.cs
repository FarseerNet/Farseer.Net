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
        T Resolve<T>(string name = "");

        /// <summary>
        ///     获取实例
        /// </summary>
        T Resolve<T>(Type type, string name = "");

        /// <summary>
        ///     获取实例
        /// </summary>
        object Resolve(Type type, string name = "");

        /// <summary>
        ///     获取所有实例
        /// </summary>
        T[] ResolveAll<T>();

        /// <summary>
        ///     获取所有实例
        /// </summary>
        object[] ResolveAll(Type type);

        /// <summary>
        ///     释放对象
        /// </summary>
        void Release(object obj);

        /// <summary>
        ///     是否注册
        /// </summary>
        bool IsRegistered(Type type);

        /// <summary>
        ///     是否注册
        /// </summary>
        bool IsRegistered<T>();
    }
}