using System;

namespace Farseer.Net.Configuration
{
    /// <summary>
    /// 配置接口，用来进行自定义配置
    /// </summary>
    public interface IDictionaryBasedConfig
    {
        /// <summary>
        /// 根据给定名称设置配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void Set<T>(string name, T value);

        /// <summary>
        /// 根据给定名称获取配置值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object Get(string name);

        /// <summary>
        /// 根据给定名称获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name);

        /// <summary>
        /// 根据给定名称获取配置值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object Get(string name, object defaultValue);

        /// <summary>
        /// 根据给定名称获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T Get<T>(string name, T defaultValue);

        /// <summary>
        /// 根据给定名称获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        T GetOrCreate<T>(string name, Func<T> creator);
    }
}