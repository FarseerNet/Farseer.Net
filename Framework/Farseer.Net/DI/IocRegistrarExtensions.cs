using System;

namespace FS.DI
{
    /// <summary>
    ///     依赖注册接口
    /// </summary>
    public static class IocRegistrarExtensions
    {
        #region RegisterIfNot
        /// <summary>
        ///     没注册则去注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iocRegistrar"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static bool RegisterIfNot<T>(this IIocRegistrar iocRegistrar, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class
        {
            if (iocRegistrar.IsRegistered<T>()) return false;

            iocRegistrar.Register<T>("", lifeStyle);
            return true;
        }

        /// <summary>
        ///     没注册则去注册
        /// </summary>
        /// <param name="iocRegistrar"></param>
        /// <param name="type"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static bool RegisterIfNot(this IIocRegistrar iocRegistrar, Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type)) return false;

            iocRegistrar.Register(type, "", lifeStyle);
            return true;
        }

        /// <summary>
        ///     没注册则去注册
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="iocRegistrar"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static bool RegisterIfNot<TType, TImpl>(this IIocRegistrar iocRegistrar, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class where TImpl : class, TType
        {
            if (iocRegistrar.IsRegistered<TType>()) return false;

            iocRegistrar.Register<TType, TImpl>("", lifeStyle);
            return true;
        }

        /// <summary>
        ///     没注册则去注册
        /// </summary>
        /// <param name="iocRegistrar"></param>
        /// <param name="type"></param>
        /// <param name="impl"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static bool RegisterIfNot(this IIocRegistrar iocRegistrar, Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type)) return false;

            iocRegistrar.Register(type, impl, "", lifeStyle);
            return true;
        }
        #endregion
    }
}