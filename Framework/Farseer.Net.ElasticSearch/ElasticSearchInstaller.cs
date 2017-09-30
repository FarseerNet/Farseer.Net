using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Farseer.Net.Configuration;
using Farseer.Net.DI;
using Farseer.Net.ElasticSearch.Configuration;

namespace Farseer.Net.ElasticSearch
{
    public class ElasticSearchInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public ElasticSearchInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 通过IOC注册ES管理接口
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var localConfigResolver = IocManager.Instance.Resolve<IConfigResolver>();
            if (localConfigResolver.ElasticSearchConfig().Items.Count == 0) { return; }

            localConfigResolver.ElasticSearchConfig().Items.ForEach(m =>
            {
                // 注册ES连接
                container.Register(
                    Component.For<IElasticSearchManager>()
                        .Named(m.Name)
                        .ImplementedBy<ElasticSearchManager>()
                        .DependsOn(Dependency.OnValue(m.GetType(), m)).LifestyleSingleton());
            });
        }
    }
}
