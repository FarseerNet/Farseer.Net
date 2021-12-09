using System;

namespace FS.EventBus.Attr
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
    public class ConsumerAttribute : Attribute
    {
        /// <summary>
        ///     是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        ///     事件名称
        /// </summary>
        public string EventName { get; set; }
    }
}