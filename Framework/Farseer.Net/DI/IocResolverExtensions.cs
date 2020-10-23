using System;

namespace FS.DI
{
    /// <summary>
    ///     IIocResolver接口扩展方法
    /// </summary>
    public static class IocResolverExtensions
    {
        /// <summary>
        ///     获取实现IDisabled接口的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iocResolver"></param>
        /// <returns></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver) { return new DisposableDependencyObjectWrapper<T>(iocResolver, iocResolver.Resolve<T>()); }

        /// <summary>
        ///     获取实现IDisabled接口的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iocResolver"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, Type type) { return new DisposableDependencyObjectWrapper<T>(iocResolver, (T)iocResolver.Resolve(type)); }

        /// <summary>
        ///     获取实现IDisabled接口的对象
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDisposableDependencyObjectWrapper ResolveAsDisposable(this IIocResolver iocResolver, Type type) { return new DisposableDependencyObjectWrapper(iocResolver, iocResolver.Resolve(type)); }

        /// <summary>
        ///     获取对象并自动释放
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iocResolver"></param>
        /// <param name="action"></param>
        public static void Using<T>(this IIocResolver iocResolver, Action<T> action)
        {
            using (var wrapper = new DisposableDependencyObjectWrapper<T>(iocResolver, iocResolver.Resolve<T>())) { action(wrapper.Object); }
        }
    }
}