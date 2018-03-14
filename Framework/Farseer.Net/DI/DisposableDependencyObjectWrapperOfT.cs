namespace FS.DI
{
    /// <summary>
    ///     包装一个从Ioc容器中获取的对象的类
    ///     此对象继承自IDisposable
    /// </summary>
    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>, IDisposableDependencyObjectWrapper
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="obj"></param>
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object obj) : base(iocResolver, obj) { }
    }

    /// <summary>
    ///     包装一个从Ioc容器中获取的对象的泛型类
    ///     此对象继承自IDisposable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        /// <param name="obj"></param>
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, T obj)
        {
            _iocResolver = iocResolver;
            Object = obj;
        }

        public T Object { get; }

        /// <summary>
        ///     释放对象
        /// </summary>
        public void Dispose() { _iocResolver.Release(Object); }
    }
}