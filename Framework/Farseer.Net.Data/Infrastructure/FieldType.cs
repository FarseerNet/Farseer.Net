using System.ComponentModel.DataAnnotations;

namespace FS.Data.Infrastructure
{
    /// <summary> 字段类型 </summary>
    public enum FieldType
    {
        /// <summary> 整型 </summary>
        [Display(Name = "Int")]
        Int,

        /// <summary> 布尔型 </summary>
        [Display(Name = "Bit")]
        Bit,

        /// <summary> 可变字符串 </summary>
        [Display(Name = "Varchar")]
        Varchar,

        /// <summary> 可变字符串（双字节） </summary>
        [Display(Name = "Nvarchar")]
        Nvarchar,

        /// <summary> 不可变字符串 </summary>
        [Display(Name = "Char")]
        Char,

        /// <summary> 不可变字符串（双字节） </summary>
        [Display(Name = "NChar")]
        NChar,

        /// <summary> 不可变文本 </summary>
        [Display(Name = "Text")]
        Text,

        /// <summary> 不可变文本 </summary>
        [Display(Name = "Ntext")]
        Ntext,

        /// <summary> 日期 </summary>
        [Display(Name = "DateTime")]
        DateTime,

        /// <summary> 短整型 </summary>
        [Display(Name = "Smallint")]
        Smallint,

        /// <summary> 浮点 </summary>
        [Display(Name = "Float")]
        Float
    }
}