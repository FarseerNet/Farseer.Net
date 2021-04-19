using System;

namespace FS.MQ.Rabbit.Attr
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumerAttribute : Attribute
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
        /// 是否自动创建交换器、队列并绑定（默认true）
        /// </summary>
        public bool AutoCreateAndBind { get; set; } = true;

        /// <summary>
        /// AutoCreateAndBind=true时，会创建ExchangeName
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// AutoCreateAndBind=true时，会创建QueueName，并绑定到ExchangeName
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 指定接收的路由KEY（默认为空）
        /// </summary>
        public string RoutingKey { get; set; } = "";

        /// <summary> 线程数 </summary>
        public int ConsumeThreadNums { get; set; } = 8;

        /// <summary> 最后ACK多少秒超时则重连（默认5分钟） </summary>
        public int LastAckTimeoutRestart { get; set; } = 5 * 60;
    }
}