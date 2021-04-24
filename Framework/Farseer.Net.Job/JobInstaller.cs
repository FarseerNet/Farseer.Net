using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.Job.Configuration;
using Microsoft.Extensions.Configuration;

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
            var jobItemConfig = container.Resolve<IConfigurationRoot>().GetSection("Job").Get<JobItemConfig>();
            //container.Register(Component.For<ServiceRegister>().Instance(new ServiceRegister(SnowflakeId.GenerateId().ToString(), jobItemConfig)).LifestyleSingleton());
            container.Register(Component.For<ServiceRegister>().DependsOn(Dependency.OnValue<string>(SnowflakeId.GenerateId().ToString()), Dependency.OnValue<JobItemConfig>(jobItemConfig)).LifestyleSingleton());
        }
    }
}