namespace FS.Data.Features {
    /// <summary>
    ///     逻辑删除的字段类型
    /// </summary>
    public enum eumSortDeleteType
    {
        /// <summary>
        ///     布尔值区分
        /// </summary>
        Bool,

        /// <summary>
        ///     数字区分
        /// </summary>
        Number,

        /// <summary>
        ///     时间区分（时间类型会赋值为当前时间）
        /// </summary>
        DateTime
    }
}