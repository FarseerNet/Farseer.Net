using System;
using CacheManager.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Farseer.Net.Cache.Configuration;
using Farseer.Net.Configuration;
using Farseer.Net.DI;
using Farseer.Net.Cache;

namespace Farseer.Net.Cache
{
    /// <summary>
    /// Redis依赖注册
    /// </summary>
    public class CacheManagerInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public CacheManagerInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var localConfigResolver = IocManager.Instance.Resolve<IConfigResolver>();
            if (localConfigResolver.CacheManagerConfig().Items.Count == 0) { return; }

            localConfigResolver.CacheManagerConfig().Items.ForEach(m =>
            {
                Action<ConfigurationBuilderCachePart> settings = null;
                switch (m.CacheModel)
                {
                    case EumCacheModel.Runtime: { settings = (o) => o.WithMemoryCacheHandle("handleName"); break; }
                    default: return;
                }

                // 注册
                container.Register(Component.For<ICacheManager>().Named(m.Name).ImplementedBy<CacheManager>().DependsOn(Dependency.OnValue(settings.GetType(), settings)).LifestyleSingleton());
            });
        }
    }
}
