using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.Job.Abstract;
using FS.Job.Configuration;
using FS.Job.GrpcClient;
using FS.Job.RemoteCall;
using FS.Reflection;
using Microsoft.Extensions.Configuration;

namespace FS.Job
{
    public class JobInstaller : IWindsorInstaller
    {
        public static Dictionary<string, Type> JobImpList = new();

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
            container.Register(Component.For<ChannelClient>().DependsOn(Dependency.OnValue<string>(SnowflakeId.GenerateId().ToString()), Dependency.OnValue<JobItemConfig>(jobItemConfig)).LifestyleTransient());

            // 业务job
            var types = container.Resolve<IAssemblyFinder>().GetType<IFssJob>();
            foreach (var jobType in types)
            {
                var fssJobAttribute = jobType.GetCustomAttribute<FssJobAttribute>();
                if (fssJobAttribute == null || !fssJobAttribute.Enable) return;
                
                // 把找到的JOB实现，存到字典中，用于向服务端注册时，告知当前客户端能处理的JOB列表
                JobImpList[fssJobAttribute.Name] = jobType;
                container.Register(Component.For<IFssJob>().ImplementedBy(jobType).Named($"fss_job_{fssJobAttribute.Name}").LifestyleTransient());
            }

            container.Register(Component.For<IRemoteCommand, PrintCommand>().Named("fss_client_Print").LifestyleTransient());
            container.Register(Component.For<IRemoteCommand, IgnoreCommand>().Named("fss_client_Ignore").LifestyleTransient());
            container.Register(Component.For<IRemoteCommand, JobSchedulerCommand>().Named("fss_client_JobScheduler").LifestyleTransient());
        }
    }
}