using System;

namespace FS.Core.Mapping.Attribute
{
    /// <summary>
    ///     设置字段在数据库中的映射关系
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public sealed class AppConfigAttribute : System.Attribute
    {
        /// <summary>
        ///     数据库字段名称（映射）
        /// </summary>
        public string Name { get; set; }
    }
}