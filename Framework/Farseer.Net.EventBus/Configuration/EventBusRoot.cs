using Collections.Pooled;
using FS.DI;
using Microsoft.Extensions.Configuration;

namespace FS.EventBus.Configuration
{
    /// <summary>
    ///     读取配置
    /// </summary>
    public class EventBusRoot
    {
        /// <summary>
        ///     读取配置
        /// </summary>
        public static EventBusConfig Get()
        {
            using var configs = IocManager.GetService<IConfigurationRoot>().GetSection(key: "EventBus").GetChildren().ToPooledList();
            var config  = new EventBusConfig() { Events = new() };
            foreach (var configurationSection in configs)
            {
                config.Events.Add(new EventConfig
                {
                    EventName = configurationSection.Key
                });
            }
            return config;
        }
    }
}