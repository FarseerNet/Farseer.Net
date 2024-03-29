﻿using System;

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
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver) => new DisposableDependencyObjectWrapper<T>(iocResolver: iocResolver, obj: iocResolver.Resolve<T>());

        /// <summary>
        ///     获取实现IDisabled接口的对象
        /// </summary>
        public static IDisposableDependencyObjectWrapper<T> ResolveAsDisposable<T>(this IIocResolver iocResolver, Type type) => new DisposableDependencyObjectWrapper<T>(iocResolver: iocResolver, obj: (T)iocResolver.Resolve(type: type));

        /// <summary>
        ///     获取实现IDisabled接口的对象
        /// </summary>
        public static IDisposableDependencyObjectWrapper ResolveAsDisposable(this IIocResolver iocResolver, Type type) => new DisposableDependencyObjectWrapper(iocResolver: iocResolver, obj: iocResolver.Resolve(type: type));

        /// <summary>
        ///     获取对象并自动释放
        /// </summary>
        public static void Using<T>(this IIocResolver iocResolver, Action<T> action)
        {
            using (var wrapper = new DisposableDependencyObjectWrapper<T>(iocResolver: iocResolver, obj: iocResolver.Resolve<T>()))
            {
                action(obj: wrapper.Object);
            }
        }
    }
}