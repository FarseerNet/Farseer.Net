using System;

namespace FS.Mapper
{
    /// <summary>
    ///     实体自动匹配特性
    /// </summary>
    public class MapAttribute : Attribute
    {
        /// <summary>
        ///     实体自动匹配特性
        /// </summary>
        public MapAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        /// <summary>
        ///     目标类型
        /// </summary>
        public Type[] TargetTypes { get; }

        /// <summary>
        ///     实体匹配方向为双向匹配
        /// </summary>
        internal virtual EumMapDirection Direction => EumMapDirection.From | EumMapDirection.To;
    }
}