using System;

namespace FS.MQ.Queue.Attr
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary> 队列名称 </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}