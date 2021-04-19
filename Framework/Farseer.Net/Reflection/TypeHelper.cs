using System;
using System.Reflection;

namespace FS.Reflection
{
    /// <summary>
    ///     类型帮助类
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        ///     是否Func类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsFunc(object obj)
        {
            if (obj == null) return false;

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType) return false;

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        /// <summary>
        ///     是否Func类型
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsFunc<TReturn>(object obj) => (obj != null) && (obj.GetType() == typeof(Func<TReturn>));

        /// <summary>
        ///     是否基元类型，包括Nullable类型
        /// </summary>
        public static bool IsPrimitiveExtendedIncludingNullable(Type type)
        {
            if (IsPrimitiveExtended(type)) return true;

            if (type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return IsPrimitiveExtended(type.GenericTypeArguments[0]);
            }
            return false;
        }

        /// <summary>
        ///     是否基元类型
        /// </summary>
        private static bool IsPrimitiveExtended(Type type)
        {
            if (type.GetTypeInfo().IsPrimitive) return true;

            return (type == typeof(string)) || (type == typeof(decimal)) || (type == typeof(DateTime)) || (type == typeof(DateTimeOffset)) || (type == typeof(TimeSpan)) || (type == typeof(Guid));
        }
    }
}