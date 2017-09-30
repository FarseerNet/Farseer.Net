using System;

namespace Farseer.Net.DI
{
    /// <summary>
    ///     包装一个从Ioc容器中获取的对象的泛型接口
    ///     此对象继承自IDisposable
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public interface IDisposableDependencyObjectWrapper<out T> : IDisposable
    {
        /// <summary>
        ///     获取对象
        /// </summary>
        T Object { get; }
    }
}