namespace FS.MQ.RabbitMQ.Configuration
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
    }
}