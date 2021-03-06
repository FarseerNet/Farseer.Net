using System;

// ReSharper disable once CheckNamespace
namespace FS.Job
{
    /// <summary>
    /// 标记Main方法启用消费
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FssAttribute : Attribute
    {
        /// <summary>
        /// 是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}