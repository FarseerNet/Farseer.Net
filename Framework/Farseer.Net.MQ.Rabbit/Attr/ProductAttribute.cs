using System;

namespace FS.MQ.Rabbit.Attr
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ProductAttribute : Attribute
    {
        /// <summary>
        /// 是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// Connect配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 交换器类型
        /// </summary>
        public eumExchangeType ExchangeType { get; set; }

        /// <summary>
        /// 是否自动创建交换器
        /// </summary>
        public bool AutoCreateExchange { get; set; } = true;

        /// <summary>
        /// AutoCreateAndBind=true时，会创建ExchangeName
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// 指定接收的路由KEY（默认为空）
        /// </summary>
        public string RoutingKey { get; set; } = "";

        /// <summary> 使用确认模式（默认关闭） </summary>
        public bool UseConfirmModel { get; set; } = true;
    }
}