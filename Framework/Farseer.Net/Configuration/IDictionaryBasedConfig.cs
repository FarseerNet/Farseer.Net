using System;

namespace FS.Configuration
{
    /// <summary>
    ///     ���ýӿڣ����������Զ�������
    /// </summary>
    public interface IDictionaryBasedConfig
    {
        /// <summary>
        ///     ���ݸ���������������ֵ
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="name"> </param>
        /// <param name="value"> </param>
        void Set<T>(string name, T value);

        /// <summary>
        ///     ���ݸ������ƻ�ȡ����ֵ
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        object Get(string name);

        /// <summary>
        ///     ���ݸ������ƻ�ȡ����ֵ
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="name"> </param>
        /// <returns> </returns>
        T Get<T>(string name);

        /// <summary>
        ///     ���ݸ������ƻ�ȡ����ֵ
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="defaultValue"> </param>
        /// <returns> </returns>
        object Get(string name, object defaultValue);

        /// <summary>
        ///     ���ݸ������ƻ�ȡ����ֵ
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="name"> </param>
        /// <param name="defaultValue"> </param>
        /// <returns> </returns>
        T Get<T>(string name, T defaultValue);

        /// <summary>
        ///     ���ݸ������ƻ�ȡ����ֵ
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="name"> </param>
        /// <param name="creator"> </param>
        /// <returns> </returns>
        T GetOrCreate<T>(string name, Func<T> creator);
    }
}