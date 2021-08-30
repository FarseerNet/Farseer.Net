using System.Collections.Generic;
using System.Linq;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Job.Configuration
{
    /// <summary>
    /// 读取Job配置
    /// </summary>
    public class JobConfigRoot
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        public static JobItemConfig Get()
        {
            var configurationSection = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("FSS");
            return configurationSection.Get<JobItemConfig>();
        }
    }
}