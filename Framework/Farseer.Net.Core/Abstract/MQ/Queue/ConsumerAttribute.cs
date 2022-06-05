using System;

namespace FS.Core.Abstract.MQ.Queue
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary>
        ///     队列名称
        /// 如果配置中不包含此Name的队列配置，则以当前消费的设置为设置。
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;
        
        /// <summary>
        ///     每次拉取的队列数量,默认1000条为一组。
        /// </summary>
        public int PullCount { get; set; } = 1000;
        /// <summary>
        ///     队列允许写入的最大数量，超出后，不再写入队列中，默认100万。0：不限制
        /// </summary>
        public int MaxCount { get; set; } = 1000000;
        /// <summary>
        ///     批量拉取消息时的间隔休眠时间（默认200ms)
        /// </summary>
        public int SleepTime { get; set; } = 200;
    }
}