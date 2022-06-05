using System;

namespace FS.Core.Abstract.MQ.RedisStream
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary>
        ///     是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        ///     Redis配置名称
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        ///     消费组（为空时，走非消费组）
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        ///     队列名称
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        ///     每次拉取数据的数量（默认1）
        /// </summary>
        public int PullCount { get; set; } = 1;

        /// <summary>
        ///     是否自动创建消费组（默认true）
        /// </summary>
        public bool AutoCreateGroup { get; set; } = true;

        /// <summary>
        ///     线程数
        /// </summary>
        public int ConsumeThreadNums { get; set; } = 8;

        /// <summary>
        ///     最后ACK多少秒超时则重连（默认5分钟）
        /// </summary>
        public int LastAckTimeoutRestart { get; set; } = 5 * 60;
    }
}