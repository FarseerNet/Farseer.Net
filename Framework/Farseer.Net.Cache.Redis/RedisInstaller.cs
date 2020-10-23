using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.DI;

namespace FS.Cache.Redis
{
    /// <summary>
    /// Redis依赖注册
    /// </summary>
    public class RedisInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public RedisInstaller(IIocResolver iocResolver)
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
            InitRedisConfig(container, localConfigResolver);
        }

        private void InitRedisConfig(IWindsorContainer container, IConfigResolver localConfigResolver)
        {
            if (localConfigResolver.RedisConfig().Items.Count == 0) { return; }

            localConfigResolver.RedisConfig().Items.ForEach(m =>
            {
                // 注册Redis连接
                container.Register(Component.For<IRedisConnectionWrapper>().Named($"{m.Name}_connection").ImplementedBy<RedisConnectionWrapper>().DependsOn(Dependency.OnValue(m.GetType(), m)).LifestyleSingleton());

                // 注册Redis管理
                container.Register(Component.For<IRedisCacheManager>().Named(m.Name).ImplementedBy<RedisCacheManager>().DependsOn(Dependency.OnValue(m.GetType(), m), Dependency.OnValue(typeof(IRedisConnectionWrapper), _iocResolver.Resolve<IRedisConnectionWrapper>($"{m.Name}_connection"))).LifestyleSingleton());
            });
        }
    }
}
