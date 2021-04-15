using System.ComponentModel.DataAnnotations;
using FS.Core.Mapping.Attribute;

namespace FS.Core.Mapping
{
    /// <summary>
    ///     保存字段映射的信息
    /// </summary>
    public class FieldMapState
    {
        /// <summary>
        ///     字符串长度
        /// </summary>
        public StringLengthAttribute StringLength { get; set; }

        /// <summary>
        ///     是否必填
        /// </summary>
        public RequiredAttribute Required { get; set; }

        /// <summary>
        ///     属性中文描述
        /// </summary>
        public DisplayAttribute Display { get; set; }

        /// <summary>
        ///     值的长度
        /// </summary>
        public RangeAttribute Range { get; set; }

        /// <summary>
        ///     正则
        /// </summary>
        public RegularExpressionAttribute RegularExpression { get; set; }
    }
}