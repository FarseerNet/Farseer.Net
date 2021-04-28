﻿using System;
using System.Linq;
using System.Reflection;
using FS.DI;
using FS.Job.Attr;
using FS.Job.Configuration;
using FS.Job.GrpcClient;
using FS.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    public class JobModule : FarseerModule
    {
        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }

        public override void PostInitialize()
        {
            var fssAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<FssAttribute>();
            if (fssAttribute is {Enable: true})
            {
                // 注册到服务端
                var serviceRegister = IocManager.Resolve<ChannelClient>();
                serviceRegister.Channel(JobInstaller.JobImpList.Keys.Select(o => o).ToArray());
            }
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new JobInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
        }
    }
}