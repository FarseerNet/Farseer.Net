using System;

namespace FS.MQ.RedisStream.Attr
{
    /// <summary>
    /// 标记Main方法启用消费
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RedisStreamAttribute : Attribute
    {
        /// <summary>
        /// 是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}