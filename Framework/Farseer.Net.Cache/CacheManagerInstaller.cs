using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;

namespace FS.Cache
{
    /// <summary>
    ///     Redis依赖注册
    /// </summary>
    public class CacheManagerInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public CacheManagerInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     注册依赖
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 注册内存缓存
            container.Register(Component.For<IGetCache>().ImplementedBy<GetCacheInMemory>().Named(name: "GetCacheInMemory").LifestyleSingleton());
            // 注册缓存管理
            container.Register(Component.For<ICacheManager>().ImplementedBy<CacheManager>().LifestyleSingleton());
        }
    }
}