using FS.Core.Mapping.Attribute;

namespace FS.Core.Mapping
{
    /// <summary>
    /// 数据库的字段映射
    /// </summary>
    public class DbFieldMapState : FieldMapState
    {
        /// <summary>
        ///     数据库字段设置
        /// </summary>
        public FieldAttribute Field { get; set; }
    }
}