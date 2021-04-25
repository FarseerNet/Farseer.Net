using System;

namespace FS.Job.Attr
{
    /// <summary>
    /// JOB配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FssJobAttribute : Attribute
    {
        /// <summary>
        /// 是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 在FSS平台配置的JobTypeName
        /// </summary>
        public string Name { get; set; }
    }
}