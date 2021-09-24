using System;
using FS.DI;

namespace FS.Configuration.Startup
{
    /// <summary>
    ///     IFarseerStartupConfiguration扩展
    /// </summary>
    public static class FarseerStartupConfigurationExtensions
    {
        /// <summary>
        ///     替换服务类型
        /// </summary>
        /// <param name="configuration"> 配置 </param>
        /// <param name="type"> 基类型 </param>
        /// <param name="impl"> 实现类型 </param>
        /// <param name="lifeStyle"> 对象生命周期 </param>
        public static void ReplaceService(this IFarseerStartupConfiguration configuration, Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            configuration.ReplaceService(type: type, replaceAction: () =>
            {
                configuration.IocManager.Register(type: type, impl: impl, name: string.Empty, lifeStyle: lifeStyle);
            });
        }

        /// <summary>
        ///     替换服务类型
        /// </summary>
        /// <typeparam name="TType"> 基类型 </typeparam>
        /// <typeparam name="TImpl"> 实现类型 </typeparam>
        /// <param name="configuration"> 配置 </param>
        /// <param name="lifeStyle"> 对象生命周期 </param>
        public static void ReplaceService<TType, TImpl>(this IFarseerStartupConfiguration configuration, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            configuration.ReplaceService(type: typeof(TType), replaceAction: () =>
            {
                configuration.IocManager.Register<TType, TImpl>(name: string.Empty, lifeStyle: lifeStyle);
            });
        }
    }
}