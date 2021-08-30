using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.Data.Configuration
{
    /// <summary>
    /// 读取Data配置
    /// </summary>
    public class DataConfigRoot
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        public static DbConfig Get()
        {
            var configurationSection = IocManager.Instance.Resolve<IConfigurationRoot>().GetSection("Database");
            return configurationSection.Get<DbConfig>();
            
        }
    }
}