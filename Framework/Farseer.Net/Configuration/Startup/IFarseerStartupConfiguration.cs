using System;
using FS.DI;

namespace FS.Configuration.Startup
{
    /// <summary>
    /// 系统启动配置接口
    /// </summary>
    public interface IFarseerStartupConfiguration : IDictionaryBasedConfig
    {
        /// <summary>
        /// 依赖注入管理器
        /// </summary>
        IIocManager IocManager { get; }

        /// <summary>
        /// 替换服务类型
        /// </summary>
        /// <param name="type">要替换的类型</param>
        /// <param name="replaceAction">替换操作</param>
        void ReplaceService(Type type, Action replaceAction);

        /// <summary>
        /// 获取一个配置对象
        /// </summary>
        T Get<T>();
    }
}