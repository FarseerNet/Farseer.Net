using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.DI;
using FS.MongoDB.Configuration;

namespace FS.MongoDB
{
    public class MongoInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public MongoInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 通过IOC注册Mongo管理接口
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var localConfigResolver = IocManager.Instance.Resolve<IConfigResolver>();
            if (localConfigResolver.MongoConfig().Items.Count == 0) { return; }

            localConfigResolver.MongoConfig().Items.ForEach(m =>
            {
                // 注册ES连接
                container.Register(
                    Component.For<IMongoManager>()
                        .Named(m.Name)
                        .ImplementedBy<MongoManager>()
                        .DependsOn(Dependency.OnValue(m.GetType(), m)).LifestyleSingleton());
            });
        }
    }
}
