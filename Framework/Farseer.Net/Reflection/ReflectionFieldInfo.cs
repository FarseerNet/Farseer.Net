// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-01-12 16:07
// ********************************************

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.Reflection
{
    /// <summary>
    ///     使用表达式树的方式来赋值/读取对象的属性
    /// </summary>
    internal static class ReflectionFieldInfo
    {
        /// <summary>
        ///     要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<FieldInfo, Action<object, object>> SetCacheList = new();

        /// <summary>
        ///     要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<FieldInfo, Func<object, object>> GetCacheList = new();

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="fieldInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Action<object, object> SetValue(FieldInfo fieldInfo)
        {
            Action<object, object> val;
            if (!SetCacheList.TryGetValue(key: fieldInfo, value: out val))
            {
                var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
                var valueParam    = Expression.Parameter(type: typeof(object), name: "value");

                //((T)instance)
                var castInstanceExpression = Expression.Convert(expression: instanceParam, type: fieldInfo.DeclaringType);

                // (T)value
                var castValueExpression = Expression.Convert(expression: valueParam, type: fieldInfo.FieldType);

                //((T)instance).Field
                var propertyProperty = Expression.Field(expression: castInstanceExpression, field: fieldInfo);

                //((T)instance).Field = value
                var assignExpression = Expression.Assign(left: propertyProperty, right: castValueExpression);
                var lambdaExpression = Expression.Lambda<Action<object, object>>(body: assignExpression, instanceParam, valueParam);
                val = lambdaExpression.Compile();
                SetCacheList.TryAdd(key: fieldInfo, value: val);
            }

            return val;
        }

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="fieldInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Func<object, object> GetValue(FieldInfo fieldInfo)
        {
            Func<object, object> val;
            if (!GetCacheList.TryGetValue(key: fieldInfo, value: out val))
            {
                var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");

                //((T)instance)
                var castInstanceExpression = Expression.Convert(expression: instanceParam, type: fieldInfo.DeclaringType);

                //((T)instance).Property
                var propertyProperty = Expression.Field(expression: castInstanceExpression, field: fieldInfo);
                var unaryVal         = Expression.Convert(expression: propertyProperty, type: typeof(object));
                var lambdaExpression = Expression.Lambda<Func<object, object>>(body: Expression.Convert(expression: unaryVal, type: typeof(object)), instanceParam);
                val = lambdaExpression.Compile();
                GetCacheList.TryAdd(key: fieldInfo, value: val);
            }

            return val;
        }
    }
}