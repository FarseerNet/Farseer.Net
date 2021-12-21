using System;
using System.Data;

namespace FS.Core.Mapping.Attribute
{
    /// <summary>
    ///     设置字段在数据库中的映射关系
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public sealed class FieldAttribute : System.Attribute
    {
        /// <summary>
        ///     数据库字段名称（映射）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     是否为数据库主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        ///     是否为数据库的标识字段（自动增长）
        /// </summary>
        public bool IsDbGenerated { get; set; }

        /// <summary>
        ///     插入时字段状态（默认允许修改）
        /// </summary>
        public StatusType InsertStatusType { get; set; }

        /// <summary>
        ///     修改时字段状态（默认允许修改）
        /// </summary>
        public StatusType UpdateStatusType { get; set; }

        /// <summary>
        ///     指定对应的数据库字段类型
        /// </summary>
        public DbType DbType { get; set; } = DbType.Object;

        /// <summary>
        ///     指定对应的数据库字段长度
        /// </summary>
        public int FieldLength { get; set; }

        /// <summary>
        ///     字段在数据库的位置（SqlBulkCopy时用到）
        /// </summary>
        public int FieldIndex { get; set; }

        // /// <summary>
        // ///     是否映射到数据库字段中(默认为true)
        // /// </summary>
        // public bool IsMap { get; set; } = true;

        /// <summary>
        ///     数据库使用json格式存储
        /// </summary>
        public EumStorageType StorageType { get; set; } = EumStorageType.Direct;

        /// <summary>
        ///     是字段还是组合字段或数据库函数(默认为false)
        /// </summary>
        public bool IsFun { get; set; }

        /// <summary>
        ///     指示字段是否为存储过程中输出的参数
        ///     （默认为false)
        /// </summary>
        public bool IsOutParam { get; set; } = false;

        /// <summary>
        ///     指示字段是否为存储过程中输入的参数
        ///     （默认为false)
        /// </summary>
        public bool IsInParam { get; set; }
    }
    
    public enum EumStorageType
    {
        /// <summary>
        /// 直接存储
        /// </summary>
        Direct,
        /// <summary>
        /// 不存储（不与数据库字段映射）
        /// </summary>
        Ignore,
        /// <summary>
        /// 使用json格式
        /// </summary>
        Json,
        /// <summary>
        /// 用,号分隔的数组
        /// </summary>
        Array
    }
}