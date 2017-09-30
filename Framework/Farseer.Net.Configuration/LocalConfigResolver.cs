// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 11:17
// ********************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Farseer.Net.Configuration.Format;

namespace Farseer.Net.Configuration
{
    /// <summary>
    ///     本地配置文件的解析
    /// </summary>
    public class LocalConfigResolver : IConfigResolver
    {
        /// <summary>
        ///     配置文件根节点、配置类型的对应关系
        /// </summary>
        private static readonly Dictionary<Type, object> ConfigBindList = new Dictionary<Type, object>();
        /// <summary>
        ///     配置文件的格式解析器
        /// </summary>
        private readonly IConfigFormat _configFormat;
        /// <summary>
        ///     配置文件
        /// </summary>
        private string ConfigFileName => $"{SysPath.AppData}{SysPath.ConfigurationName}{_configFormat.ExtensionName}";
        /// <summary>
        ///     配置的最后一次写入时间
        /// </summary>
        private DateTime _fileLastWriteTime;

        /// <summary>
        ///     本地配置文件的解析
        /// </summary>
        public LocalConfigResolver(IConfigFormat configFormat) { _configFormat = configFormat; }

        /// <summary>
        ///     追加配置
        /// </summary>
        public void Append(Type configType)
        {
            Check.NotNull(configType, nameof(configType));

            ConfigBindList[configType] = Activator.CreateInstance(configType);
        }

        /// <summary>
        ///     读取配置文件
        /// </summary>
        public void Load()
        {
            //不存在则自动接创建
            if (!File.Exists(ConfigFileName)) Save();

            // 读取配置文件到字符串
            var config = File.ReadAllText(ConfigFileName);

            // 到格式器中解析转成配置字典
            var dicConfig = _configFormat.Resolver(ConfigBindList, config);

            // 转换到基类的字典类型
            SetConfig(dicConfig);
            _fileLastWriteTime = File.GetLastWriteTime(ConfigFileName);
        }

        /// <summary>
        ///     转换到基类的字典类型
        /// </summary>
        /// <param name="dicConfig">string：TypeName，dynamic：配置实体类</param>
        private static void SetConfig(Dictionary<string, object> dicConfig)
        {
            foreach (var type in ConfigBindList.Keys.ToArray())
            {
                if (dicConfig.ContainsKey(type.Name)) ConfigBindList[type] = dicConfig[type.Name];
                else ConfigBindList[type] = Activator.CreateInstance(type);
            }
        }

        /// <inherit/>
        public void Save()
        {
            var content = _configFormat.Serialize(ConfigBindList);
            Directory.CreateDirectory(SysPath.AppData);
            File.WriteAllText(ConfigFileName, content);
        }

        /// <summary>
        ///     获取配置文件
        /// </summary>
        /// <typeparam name="TConfigEntity">继承自FarseerConfig的自定义配置</typeparam>
        public TConfigEntity Get<TConfigEntity>() where TConfigEntity : class, IFarseerConfig, new() => ConfigBindList[typeof(TConfigEntity)] as TConfigEntity ?? new TConfigEntity();

        /// <summary>
        ///     获取配置文件
        /// </summary>
        /// <typeparam name="TConfigEntity">继承自FarseerConfig的自定义配置</typeparam>
        public void Set<TConfigEntity>(TConfigEntity configEntity) where TConfigEntity : IFarseerConfig, new() => ConfigBindList[typeof(TConfigEntity)] = configEntity;
    }
}