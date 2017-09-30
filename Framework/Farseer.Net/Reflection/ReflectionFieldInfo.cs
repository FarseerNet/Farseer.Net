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
    internal static class ReflectionFieldInfo
    {
        /// <summary>
        /// 要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<FieldInfo, Action<object, object>> SetCacheList = new ConcurrentDictionary<FieldInfo, Action<object, object>>();
        /// <summary>
        /// 要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<FieldInfo, Func<object, object>> GetCacheList = new ConcurrentDictionary<FieldInfo, Func<object, object>>();

        /// <summary>
        ///     动态构造赋值委托
        /// </summary>
        /// <param name="fieldInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Action<object, object> SetValue(FieldInfo fieldInfo)
        {
            Action<object, object> val;
            if (!SetCacheList.TryGetValue(fieldInfo, out val))
            {
                var instanceParam = Expression.Parameter(typeof(object), "instance");
                var valueParam = Expression.Parameter(typeof(object), "value");

                //((T)instance)
                var castInstanceExpression = Expression.Convert(instanceParam, fieldInfo.DeclaringType);

                // (T)value
                var castValueExpression = Expression.Convert(valueParam, fieldInfo.FieldType);

                //((T)instance).Field
                var propertyProperty = Expression.Field(castInstanceExpression, fieldInfo);

                //((T)instance).Field = value
                var assignExpression = Expression.Assign(propertyProperty, castValueExpression);
                var lambdaExpression = Expression.Lambda<Action<object, object>>(assignExpression, instanceParam, valueParam);
                val = lambdaExpression.Compile();
                SetCacheList.TryAdd(fieldInfo, val);
            }
            return val;
        }

        /// <summary>
        ///     动态构造获取值委托
        /// </summary>
        /// <param name="fieldInfo">属性值类型</param>
        /// <returns>强类型委托</returns>
        public static Func<object, object> GetValue(FieldInfo fieldInfo)
        {
            Func<object, object> val;
            if (!GetCacheList.TryGetValue(fieldInfo, out val))
            {
                var instanceParam = Expression.Parameter(typeof(object), "instance");

                //((T)instance)
                var castInstanceExpression = Expression.Convert(instanceParam, fieldInfo.DeclaringType);

                //((T)instance).Property
                var propertyProperty = Expression.Field(castInstanceExpression, fieldInfo);
                var unaryVal = Expression.Convert(propertyProperty, typeof(object));
                var lambdaExpression = Expression.Lambda<Func<object, object>>(Expression.Convert(unaryVal, typeof(object)), instanceParam);
                val = lambdaExpression.Compile();
                GetCacheList.TryAdd(fieldInfo, val);
            }
            return val;
        }
    }
}