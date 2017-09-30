// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-01-12 16:07
// ********************************************

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Farseer.Net.Reflection
{
    /// <summary>
    /// 使用表达式树的方式来赋值/读取对象的属性
    /// </summary>
    internal static class ReflectionPropertyInfo
    {
        /// <summary>
        /// 要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> SetCacheList = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
        /// <summary>
        /// 要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> GetCacheList = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Action<object, object> SetValue(PropertyInfo propertyInfo)
        {
            Action<object, object> val;
            if (!SetCacheList.TryGetValue(propertyInfo, out val))
            {
                // 实体类
                var instanceParam = Expression.Parameter(typeof(object), "instance");
                // 要赋的值
                var valueParam = Expression.Parameter(typeof(object), "value");

                //((T)instance)
                var castInstanceExpression = Expression.Convert(instanceParam, propertyInfo.DeclaringType);

                // (T)value
                var castValueExpression = Expression.Convert(valueParam, propertyInfo.PropertyType);

                // 调用PropertySet方法
                var setter = propertyInfo.GetSetMethod();
                var assignExpression = Expression.Call(castInstanceExpression, setter, castValueExpression);

                var lambdaExpression = Expression.Lambda<Action<object, object>>(assignExpression, instanceParam, valueParam);
                val = lambdaExpression.Compile();
                SetCacheList.TryAdd(propertyInfo, val);
            }
            return val;
        }

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="propertyInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Func<object, object> GetValue(PropertyInfo propertyInfo)
        {
            Func<object, object> val;
            if (!GetCacheList.TryGetValue(propertyInfo, out val))
            {
                // 实体类
                var instanceParam = Expression.Parameter(typeof(object), "instance");
                //((T)instance)
                var castInstanceExpression = Expression.Convert(instanceParam, propertyInfo.DeclaringType);

                // 调用PropertyGet方法
                var getter = propertyInfo.GetGetMethod();
                var unaryVal = Expression.Call(castInstanceExpression, getter);
                var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(unaryVal, typeof(object)), instanceParam);
                val = lambdaExpression.Compile();
                GetCacheList.TryAdd(propertyInfo, val);
            }
            return val;
        }
    }
}