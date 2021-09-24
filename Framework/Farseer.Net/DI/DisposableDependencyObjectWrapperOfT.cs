namespace FS.DI
{
    /// <summary>
    ///     ��װһ����Ioc�����л�ȡ�Ķ������
    ///     �˶���̳���IDisposable
    /// </summary>
    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>, IDisposableDependencyObjectWrapper
    {
        /// <summary>
        ///     ���캯��
        /// </summary>
        /// <param name="iocResolver"> </param>
        /// <param name="obj"> </param>
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object obj) : base(iocResolver: iocResolver, obj: obj)
        {
        }
    }

    /// <summary>
    ///     ��װһ����Ioc�����л�ȡ�Ķ���ķ�����
    ///     �˶���̳���IDisposable
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     ���캯��
        /// </summary>
        /// <param name="iocResolver"> </param>
        /// <param name="obj"> </param>
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, T obj)
        {
            _iocResolver = iocResolver;
            Object       = obj;
        }

        public T Object { get; }

        /// <summary>
        ///     �ͷŶ���
        /// </summary>
        public void Dispose()
        {
            _iocResolver.Release(obj: Object);
        }
    }
}