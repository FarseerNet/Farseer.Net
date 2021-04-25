using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.Job.Attr;
using FS.Job.Configuration;
using FS.Job.GrpcClient;
using FS.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    public class JobInstaller : IWindsorInstaller
    {
        /// <summary>
        /// 依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iocResolver"></param>
        public JobInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// 初始化IOC
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var jobItemConfig = container.Resolve<IConfigurationRoot>().GetSection("FSS").Get<JobItemConfig>();

            // 服务注册
            container.Register(Component.For<ServiceRegister>().DependsOn(Dependency.OnValue<string>(SnowflakeId.GenerateId().ToString()), Dependency.OnValue<JobItemConfig>(jobItemConfig)).LifestyleSingleton());

            // 业务job
            var types = container.Resolve<IAssemblyFinder>().GetType<IFssJob>();
            foreach (var jobType in types)
            {
                var fssJobAttribute = jobType.GetCustomAttribute<FssJobAttribute>();
                if (fssJobAttribute == null || !fssJobAttribute.Enable) return;
                IocManager.Instance.Logger<JobInstaller>().LogInformation($"初始化：【{jobType.Name}】- {fssJobAttribute.Name} 任务");
                container.Register(Component.For<IFssJob>().ImplementedBy(jobType).Named(fssJobAttribute.Name));
            }
        }
    }
}