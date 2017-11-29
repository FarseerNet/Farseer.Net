using Farseer.Net.Core.Mapping.Attribute;

namespace Farseer.Net.Core.Mapping
{
    /// <summary>
    ///     保存字段映射的信息
    /// </summary>
    public class SetFieldMap
    {
        /// <summary>
        ///     数据库字段设置
        /// </summary>
        public FieldAttribute Field { get; set; }
    }
}