using System;

namespace FS.Mapper
{
    /// <summary>
    ///     实体自动匹配特性
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Field | AttributeTargets.Property)]
    public class MapFieldAttribute : Attribute
    {
        /// <summary>
        ///     是否忽略该字段的映射
        /// </summary>
        public bool IsIgnore { get; set; }

        /// <summary>
        ///     对方实际的映射的字段名
        /// </summary>
        public string FromName { get; set; }
    }
}