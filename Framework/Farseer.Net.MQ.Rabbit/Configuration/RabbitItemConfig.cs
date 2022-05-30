using System.Collections.Generic;
using Collections.Pooled;

namespace FS.MQ.Rabbit.Configuration
{
    /// <summary>
    ///     配置信息
    /// </summary>
    public class RabbitItemConfig
    {
        /// <summary> 服务端 </summary>
        public RabbitServerConfig Server { get; set; }
        
        /// <summary> 生产者配置 </summary>
        public PooledList<ProductConfig> Product { get; set; }
    }
}