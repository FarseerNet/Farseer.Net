namespace FS.Core.Mapping
{
    /// <summary> 字段状态 </summary>
    public enum SetFieldStatus
    {
        /// <summary> 允许修改 </summary>
        CanWrite,

        /// <summary> 只读状态，不作任何设置 </summary>
        ReadOnly,

        /// <summary> 只读状态，但如果存在值时，将转换成==条件 </summary>
        ReadCondition
    }
}