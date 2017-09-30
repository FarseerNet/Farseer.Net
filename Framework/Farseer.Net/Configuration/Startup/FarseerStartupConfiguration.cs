using System;
using System.Collections.Generic;
using Farseer.Net.DI;

namespace Farseer.Net.Configuration.Startup
{
    /// <summary>
    /// 系统启动配置
    /// </summary>
    public class FarseerStartupConfiguration : DictionaryBasedConfig, IFarseerStartupConfiguration
    {
        /// <summary>
        /// 依赖注入管理器
        /// </summary>
        public IIocManager IocManager { get; private set; }

        /// <summary>
        /// 默认的数据库连接字符串
        /// </summary>
        public string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        /// 模块配置
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }


        /// <summary>
        /// 需要替换服务的操作字典
        /// </summary>
        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FarseerStartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initialize()
        {
            Modules = IocManager.Resolve<IModuleConfigurations>();
            ServiceReplaceActions = new Dictionary<Type, Action>();
        }

        /// <summary>
        /// 替换服务类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="replaceAction"></param>
        public void ReplaceService(Type type, Action replaceAction)
        {
            ServiceReplaceActions[type] = replaceAction;
        }

        /// <summary>
        /// 获取一个配置对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            return GetOrCreate(typeof(T).FullName, () => IocManager.Resolve<T>());
        }
    }
}