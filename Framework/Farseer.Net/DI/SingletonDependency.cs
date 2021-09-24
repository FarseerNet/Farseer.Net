using System;

namespace FS.DI
{
    /// <summary>
    ///     单例依赖
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public static class SingletonDependency<T>
    {
        /// <summary>
        ///     延迟实例
        /// </summary>
        private static readonly Lazy<T> LazyInstance;

        /// <summary>
        ///     构造函数
        /// </summary>
        static SingletonDependency()
        {
            LazyInstance = new Lazy<T>(valueFactory: () => IocManager.Instance.Resolve<T>());
        }

        /// <summary>
        ///     获取实例
        /// </summary>
        public static T Instance => LazyInstance.Value;
    }
}