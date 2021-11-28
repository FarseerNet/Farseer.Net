using System;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Job.Configuration
{
    /// <summary>
    ///     读取Job配置
    /// </summary>
    public class JobConfigRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static JobItemConfig Get()
        {
            var configurationSection = IocManager.GetService<IConfigurationRoot>().GetSection(key: "FSS");
            var jobItemConfig        = configurationSection.Get<JobItemConfig>();

            if (jobItemConfig.PullCount == 0) jobItemConfig.PullCount = Environment.ProcessorCount;
            if (jobItemConfig.WorkCount == 0) jobItemConfig.WorkCount = Environment.ProcessorCount;
            return jobItemConfig;
        }
    }
}