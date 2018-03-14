// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 11:17
// ********************************************

using System;

namespace FS.Configuration
{
    /// <summary>
    /// 本地配置文件的解析
    /// </summary>
    public interface IConfigResolver
    {
        /// <summary>
        /// 追加绑定配置文件关系
        /// </summary>
        void Append(Type configType);

        /// <summary>
        /// 读取配置文件
        /// </summary>
        void Load();

        /// <summary>
        /// 保存配置文件
        /// </summary>
        void Save();

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <typeparam name="TConfigEntity">继承自FarseerConfig的自定义配置</typeparam>
        TConfigEntity Get<TConfigEntity>() where TConfigEntity : class, IFarseerConfig, new();

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <typeparam name="TConfigEntity">继承自FarseerConfig的自定义配置</typeparam>
        void Set<TConfigEntity>(TConfigEntity configEntity) where TConfigEntity : IFarseerConfig, new();
    }
}