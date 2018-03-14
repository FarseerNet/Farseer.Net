using System;

namespace FS.Mapper
{
    /// <summary>
    /// 实体自动匹配特性
    /// </summary>
    public class AutoMapAttribute : Attribute
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public Type[] TargetTypes { get; private set; }

        /// <summary>
        /// 实体自动匹配特性
        /// </summary>
        public AutoMapAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        /// <summary>
        /// 实体匹配方向为双向匹配
        /// </summary>
        internal virtual AutoMapDirection Direction => AutoMapDirection.From | AutoMapDirection.To;
    }
}
