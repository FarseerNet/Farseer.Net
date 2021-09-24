using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.MongoDB.Configuration;
using Microsoft.Extensions.Configuration;

namespace FS.MongoDB
{
    public class MongoInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public MongoInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     通过IOC注册Mongo管理接口
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection(key: "Mongo");
            var mongoItemConfigs     = configurationSection.GetChildren().Select(selector: o => o.Get<MongoItemConfig>()).ToList();

            mongoItemConfigs.ForEach(action: m =>
            {
                // 注册ES连接
                container.Register(
                                   Component.For<IMongoManager>()
                                            .Named(name: m.Name)
                                            .ImplementedBy<MongoManager>()
                                            .DependsOn(dependency: Dependency.OnValue(dependencyType: m.GetType(), value: m))
                                            .LifestyleSingleton());
            });
        }
    }
}