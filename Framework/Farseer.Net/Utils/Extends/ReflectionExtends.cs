using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class Extend
    {
        /// <summary>
        ///     获取当前Type实现或继承的所有接口
        /// </summary>
        /// <param name="type"> 当前的Type对象 </param>
        /// <param name="exceptInterfaces"> 过滤接口的类型数组 </param>
        /// <returns> </returns>
        public static IEnumerable<Type> GetInterfacesTypes(this Type type, params Type[] exceptInterfaces)
        {
            if (type             == null) throw new ArgumentNullException(paramName: nameof(type));
            if (exceptInterfaces == null) throw new ArgumentNullException(paramName: nameof(exceptInterfaces));

            return from interfaceType in type.GetInterfaces()
                   where !exceptInterfaces.Contains(value: interfaceType)
                   select interfaceType;
        }

        /// <summary>
        ///     获取当前Type继承链上的所有基类型
        /// </summary>
        /// <param name="type"> 当前的Type对象 </param>
        /// <returns> </returns>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if (type == null) throw new ArgumentNullException(paramName: nameof(type));
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsInterface) throw new ArgumentException(message: "接口不能获取基类。");

            for (var baseType = typeInfo.BaseType; baseType != typeof(object); baseType = baseType.GetTypeInfo().BaseType)
            {
                if (baseType == typeof(ValueType)) continue;
                yield return baseType;
            }
        }

        /// <summary>
        ///     获取当前Type继承或实现的所有泛型类型定义
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public static IEnumerable<Type> GetGenericTypeDefinitions(this Type type)
        {
            if (type == null) throw new ArgumentNullException(paramName: nameof(type));

            if (type.IsGenericTypeDefinition()) yield return type.GetGenericTypeDefinition();
            foreach (var genericType in type.GetTypeInfo().IsClass ? type.GetInterfacesTypes().Concat(second: type.GetBaseTypes()) : type.GetInterfacesTypes())
            {
                var typeInfo = genericType.GetTypeInfo();
                if (typeInfo.IsGenericTypeDefinition) yield return genericType;
                if (genericType.IsGenericTypeDefinition()) yield return genericType.GetGenericTypeDefinition();
            }
        }

        /// <summary>
        ///     指示当前 Type 是否是开放式构造类型的泛型定义。
        /// </summary>
        /// <param name="type"> 当前的Type对象 </param>
        /// <returns> </returns>
        public static bool IsGenericTypeDefinition(this Type type)
        {
            if (type == null) throw new ArgumentNullException(paramName: nameof(type));
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition;
        }

        /// <summary>
        ///     获取指定方法或构造方法的参数类型
        /// </summary>
        /// <param name="method"> </param>
        /// <returns> </returns>
        public static IEnumerable<Type> GetParameterTypes(this MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(paramName: nameof(method));
            return from parameter in method.GetParameters() select parameter.ParameterType;
        }

        /// <summary>
        ///     获取指定方法或构造方法的参数类型
        /// </summary>
        /// <param name="constructor"> </param>
        /// <returns> </returns>
        public static IEnumerable<Type> GetParameterTypes(this ConstructorInfo constructor)
        {
            if (constructor == null) throw new ArgumentNullException(paramName: nameof(constructor));
            return from parameter in constructor.GetParameters() select parameter.ParameterType;
        }
    }
}