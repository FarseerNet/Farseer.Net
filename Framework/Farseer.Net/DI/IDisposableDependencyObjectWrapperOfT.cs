using System;

namespace FS.DI
{
    /// <summary>
    ///     ��װһ����Ioc�����л�ȡ�Ķ���ķ��ͽӿ�
    ///     �˶���̳���IDisposable
    /// </summary>
    /// <typeparam name="T"> �������� </typeparam>
    public interface IDisposableDependencyObjectWrapper<out T> : IDisposable
    {
        /// <summary>
        ///     ��ȡ����
        /// </summary>
        T Object { get; }
    }
}