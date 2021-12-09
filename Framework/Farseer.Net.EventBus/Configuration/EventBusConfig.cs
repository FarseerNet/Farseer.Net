using System.Collections.Generic;

namespace FS.EventBus.Configuration
{
    /// <summary>
    ///     事件总线配置信息
    /// </summary>
    public class EventBusConfig
    {
        /// <summary> 事件配置 </summary>
        public List<EventConfig> Events { get; set; }
    }
}