using Collections.Pooled;

namespace FS.EventBus.Configuration
{
    /// <summary>
    ///     事件总线配置信息
    /// </summary>
    public class EventBusConfig
    {
        /// <summary> 事件配置 </summary>
        public PooledList<EventConfig> Events { get; set; }
    }
}