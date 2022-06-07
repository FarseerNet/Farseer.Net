using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Core.Abstract.Cache;
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
            container.Register(Component.For<ICacheServices>().ImplementedBy<CacheServices>().LifestyleSingleton());
            
            container.Register(Component.For<ICache>().ImplementedBy<CacheInMemory>().Named(name: "CacheInMemory").LifestyleSingleton());
        }
    }
}