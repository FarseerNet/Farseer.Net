
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Farseer.Net.Reflection
{
    /// <summary>
    ///     反射帮助类
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        ///     检查类型 <paramref name="givenType" /> 是否实现或者继承类型 <paramref name="genericType" />.
        /// </summary>
        /// <param name="givenType">要检查的类型</param>
        /// <param name="genericType">Generic type</param>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (givenType.GetTypeInfo().IsGenericType && (givenType.GetGenericTypeDefinition() == genericType)) return true;

            foreach (var interfaceType in givenType.GetInterfaces()) if (interfaceType.GetTypeInfo().IsGenericType && (interfaceType.GetGenericTypeDefinition() == genericType)) return true;

            if (givenType.GetTypeInfo().BaseType == null) return false;

            return IsAssignableToGenericType(givenType.GetTypeInfo().BaseType, genericType);
        }

        /// <summary>
        ///     Gets a list of attributes defined for a class member and it's declaring type including inherited attributes.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        public static List<TAttribute> GetAttributesOfMemberAndDeclaringType<TAttribute>(MemberInfo memberInfo) where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            //Add attributes on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true)) attributeList.AddRange(memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());

            //Add attributes on the class
            if ((memberInfo.DeclaringType != null) && memberInfo.DeclaringType.GetTypeInfo().IsDefined(typeof(TAttribute), true)) attributeList.AddRange(memberInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());

            return attributeList;
        }

        /// <summary>
        ///     Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        ///     Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute)) where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true)) return memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();

            //Get attribute from class
            if ((memberInfo.DeclaringType != null) && memberInfo.DeclaringType.GetTypeInfo().IsDefined(typeof(TAttribute), true)) return memberInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();

            return defaultValue;
        }

        /// <summary>
        ///     使用表达式树的方式来赋值
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <param name="instance"></param>
        /// <param name="setValue"></param>
        /// <returns>强类型委托</returns>
        public static void SetValue(PropertyInfo propertyInfo, object instance, object setValue) => ReflectionPropertyInfo.SetValue(propertyInfo)(instance, setValue);

        /// <summary>
        ///     使用表达式树的方式来读取
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <param name="instance"></param>
        /// <returns>强类型委托</returns>
        public static object GetValue(PropertyInfo propertyInfo, object instance) => ReflectionPropertyInfo.GetValue(propertyInfo)(instance);

        /// <summary>
        ///     使用表达式树的方式来赋值
        /// </summary>
        /// <param name="fieldInfo">属性值类型</param>
        /// <param name="instance"></param>
        /// <param name="setValue"></param>
        /// <returns>强类型委托</returns>
        public static void SetValue(FieldInfo fieldInfo, object instance, object setValue) => ReflectionFieldInfo.SetValue(fieldInfo)(instance, setValue);

        /// <summary>
        ///     使用表达式树的方式来读取
        /// </summary>
        /// <param name="fieldInfo">属性值类型</param>
        /// <param name="instance"></param>
        /// <returns>强类型委托</returns>
        public static object GetValue(FieldInfo fieldInfo, object instance) => ReflectionFieldInfo.GetValue(fieldInfo)(instance);
    }
}