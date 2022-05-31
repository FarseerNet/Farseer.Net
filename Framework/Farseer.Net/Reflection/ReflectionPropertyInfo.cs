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
    internal static class ReflectionPropertyInfo
    {
        /// <summary>
        ///     要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> SetCacheList = new();

        /// <summary>
        ///     要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> GetCacheList = new();

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="propertyInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Action<object, object> SetValue(PropertyInfo propertyInfo)
        {
            if (!SetCacheList.TryGetValue(key: propertyInfo, value: out var val))
            {
                // 实体类
                var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
                // 要赋的值
                var valueParam = Expression.Parameter(type: typeof(object), name: "value");

                //((T)instance)
                var castInstanceExpression = Expression.Convert(expression: instanceParam, type: propertyInfo.DeclaringType);

                // (T)value
                var castValueExpression = Expression.Convert(expression: valueParam, type: propertyInfo.PropertyType);

                // 调用PropertySet方法
                var setter           = propertyInfo.GetSetMethod();
                var assignExpression = Expression.Call(instance: castInstanceExpression, method: setter, castValueExpression);

                var lambdaExpression = Expression.Lambda<Action<object, object>>(body: assignExpression, instanceParam, valueParam);
                val = lambdaExpression.Compile();
                SetCacheList.TryAdd(key: propertyInfo, value: val);
            }

            return val;
        }

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="propertyInfo"> 属性值类型 </param>
        /// <returns> 强类型委托 </returns>
        public static Func<object, object> GetValue(PropertyInfo propertyInfo)
        {
            if (!GetCacheList.TryGetValue(key: propertyInfo, value: out var val))
            {
                // 实体类
                var instanceParam = Expression.Parameter(type: typeof(object), name: "instance");
                //((T)instance)
                var castInstanceExpression = Expression.Convert(expression: instanceParam, type: propertyInfo.DeclaringType);

                // 调用PropertyGet方法
                var getter           = propertyInfo.GetGetMethod();
                var unaryVal         = Expression.Call(instance: castInstanceExpression, method: getter);
                var lambdaExpression = Expression.Lambda<Func<object, object>>(body: Expression.Convert(expression: unaryVal, type: typeof(object)), instanceParam);
                val = lambdaExpression.Compile();
                GetCacheList.TryAdd(key: propertyInfo, value: val);
            }

            return val;
        }
    }
}