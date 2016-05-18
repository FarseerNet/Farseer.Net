using System;
using System.Collections.Generic;
using System.IO;
using FS.Utils.Common;

namespace FS.Configs
{
    /// <summary>
    ///     配置管理工具
    /// </summary>
    public abstract class AbsConfigs<T> where T : class, new()
    {
        /// <summary>
        ///     锁对象
        /// </summary>
        private static readonly object m_LockHelper = new object();

        /// <summary>
        ///     配置文件路径（自定义路径）
        /// </summary>
        private static readonly Dictionary<Type, string> dicFilePath = new Dictionary<Type, string>();

        /// <summary>
        ///     配置文件路径（默认路径在：/App_Data/中，如有多个项目使用同样的配置，可重定义路径位置）
        /// </summary>
        public static string FilePath
        {
            get { return dicFilePath.ContainsKey(typeof(T)) ? dicFilePath[typeof(T)] : SysMapPath.AppData; }
            set { dicFilePath[typeof(T)] = value; }
        }

        /// <summary>
        ///     配置文件名称
        /// </summary>
        private static string _fileName;

        /// <summary>
        ///     配置变量
        /// </summary>
        protected static T _configEntity;

        /// <summary>
        ///     Config修改时间
        /// </summary>
        private static DateTime _fileLastWriteTime;

        /// <summary>
        ///     加载配置文件的时间（60分钟重新加载）
        /// </summary>
        private static DateTime _loadTime;

        /// <summary>
        ///     获取配置文件所在路径
        /// </summary>
        private static string FileName => _fileName ?? (_fileName = $"{(typeof(T).Name.EndsWith("Config", true, null) ? typeof(T).Name.Substring(0, typeof(T).Name.Length - 6) : typeof(T).Name)}.config");

        /// <summary>
        ///     加载(反序列化)指定对象类型的配置对象
        /// </summary>
        private static void LoadConfig()
        {
            //不存在则自动接创建
            if (!File.Exists(FilePath + FileName))
            {
                var t = new T();
                //DynamicOperate.AddItem(t);
                SaveConfig(t);
            }
            _fileLastWriteTime = File.GetLastWriteTime(FilePath + FileName);

            lock (m_LockHelper)
            {
                _configEntity = Serialize.Load<T>(FilePath, FileName, true);
                _loadTime = DateTime.Now;
            }
        }

        /// <summary>
        ///     保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="t">Config配置</param>
        public static void SaveConfig(T t = null)
        {
            if (t == null) { t = ConfigEntity; }
            Serialize.Save(t, FilePath, FileName);
            _configEntity = t;
        }

        /// <summary>
        ///     配置变量
        /// </summary>
        public static T ConfigEntity
        {
            get
            {
                if (_configEntity == null || ((DateTime.Now - _loadTime).TotalMinutes > 60 && _fileLastWriteTime != File.GetLastWriteTime(FilePath + FileName))) { LoadConfig(); }
                return _configEntity;
            }
        }
    }
}