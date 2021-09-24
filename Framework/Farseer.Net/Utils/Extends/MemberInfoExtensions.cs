using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     MemberInfo扩展
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        ///     获取MemberInfo指定的特性类型
        /// </summary>
        /// <typeparam name="TAttribute"> 特性Attribute类型 </typeparam>
        /// <param name="memberInfo"> 成员 </param>
        /// <param name="inherit"> 是否包含从基类继承的特性 </param>
        public static TAttribute GetAttributeOrNull<TAttribute>(this MemberInfo memberInfo, bool inherit = true) where TAttribute : Attribute
        {
            Check.NotNull(value: memberInfo);

            var attrs = memberInfo.GetCustomAttributes(attributeType: typeof(TAttribute), inherit: inherit);
            return attrs.Length > 0 ? (TAttribute)attrs[0] : default;
        }

        /// <summary>
        ///     获取MemberInfo指定的特性类型（当前类找不到时，依次向上查找）
        /// </summary>
        /// <typeparam name="TAttribute"> 特性Attribute类型 </typeparam>
        /// <param name="type"> 当前成员的类型 </param>
        /// <param name="inherit"> 是否包含从基类继承的特性 </param>
        public static TAttribute GetAttributeOrNull<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
        {
            Check.NotNull(value: type);
            var attr = GetAttributeOrNull<TAttribute>(memberInfo: type, inherit: inherit);
            if (attr != null) return attr;
            return type.BaseType == null ? null : GetAttributeOrNull<TAttribute>(type: type.BaseType, inherit: inherit);
        }
    }
}