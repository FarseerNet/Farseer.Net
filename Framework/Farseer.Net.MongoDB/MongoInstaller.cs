using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration;
using FS.DI;
using FS.MongoDB.Configuration;
using Microsoft.Extensions.Configuration;

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
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("Mongo");
            var mongoItemConfigs     = configurationSection.GetChildren().Select(o => o.Get<MongoItemConfig>()).ToList();

            mongoItemConfigs.ForEach(m =>
            {
                // 注册ES连接
                container.Register(
                    Component.For<IMongoManager>()
                        .Named($"mongo_{m.Name}")
                        .ImplementedBy<MongoManager>()
                        .DependsOn(Dependency.OnValue(m.GetType(), m)).LifestyleSingleton());
            });
        }
    }
}
