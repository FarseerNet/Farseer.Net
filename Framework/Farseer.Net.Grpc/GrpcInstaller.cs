using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Farseer.Net.Grpc.Configuration;
using Microsoft.Extensions.Configuration;

namespace Farseer.Net.Grpc
{
    /// <summary>
    ///     IOC注册
    /// </summary>
    public class GrpcInstaller : IWindsorInstaller
    {
        /// <inheritdoc />
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 读取配置
            var configurationSection = container.Resolve<IConfigurationRoot>().GetSection("Grpc");
            var grpcItemConfigs      = configurationSection.GetChildren().Select(o => o.Get<GrpcItemConfig>()).ToList();

            foreach (var grpcItemConfig in grpcItemConfigs)
            {
                container.Register(Component.For<IGrpcClient>().Named($"grpc_{grpcItemConfig.Name}").ImplementedBy<GrpcClient>().DependsOn(Dependency.OnValue<GrpcItemConfig>(grpcItemConfig)).LifestyleSingleton());
            }
        }
    }
}