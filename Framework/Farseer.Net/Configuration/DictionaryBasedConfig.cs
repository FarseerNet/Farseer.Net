using System;
using System.Collections.Generic;

namespace FS.Configuration
{
    /// <summary>
    ///     配置基类，用来进行自定义配置
    /// </summary>
    public class DictionaryBasedConfig : IDictionaryBasedConfig
    {
        /// <summary>
        ///     自定义配置字典
        /// </summary>
        protected Dictionary<string, object> CustomSettings { get; }

        /// <summary>
        ///     获取一个配置值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get => CustomSettings.ContainsKey(name) ? CustomSettings[name] : null;
            set => CustomSettings[name] = value;
        }

        /// <summary>
        ///     构造函数
        /// </summary>
        protected DictionaryBasedConfig() { CustomSettings = new Dictionary<string, object>(); }

        /// <summary>
        ///     根据给定类型获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name)
        {
            var value = this[name];
            return value == null ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        ///     根据给定名称设置配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set<T>(string name, T value) { this[name] = value; }

        /// <summary>
        ///     根据给定名称获取配置值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Get(string name) { return Get(name, null); }

        /// <summary>
        ///     根据给定名称获取配置值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object Get(string name, object defaultValue)
        {
            var value = this[name];
            if (value == null) return defaultValue;

            return this[name];
        }

        /// <summary>
        ///     根据给定名称获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T Get<T>(string name, T defaultValue) { return (T)Get(name, (object)defaultValue); }

        /// <summary>
        ///     根据给定名称获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public T GetOrCreate<T>(string name, Func<T> creator)
        {
            var value = Get(name);
            if (value == null)
            {
                value = creator();
                Set(name, value);
            }
            return (T)value;
        }
    }
}