using FS.Core.Abstract.MQ.Rabbit;

namespace FS.MQ.Rabbit.Configuration
{
    public class ProductConfig
    {
        /// <summary> 配置名称 </summary>
        public string Name { get; set; }

        /// <summary> 交换器名称 </summary>
        public string ExchangeName { get; set; }

        /// <summary> 路由Key </summary>
        public string RoutingKey { get; set; }

        /// <summary> 使用确认模式（默认关闭） </summary>
        public bool UseConfirmModel { get; set; }

        /// <summary> 是否自动创建交换器 </summary>
        public bool AutoCreateExchange { get; set; }

        /// <summary> 交换器类型 </summary>
        public eumExchangeType ExchangeType { get; set; }

        /// <summary> 最低频道连接池（推荐设为8） </summary>
        public int MinFreeChannelPool { get; set; }

        /// <summary> 最大频道连接池（推荐设为10） </summary>
        public int MaxFreeChannelPool { get; set; }
    }
}