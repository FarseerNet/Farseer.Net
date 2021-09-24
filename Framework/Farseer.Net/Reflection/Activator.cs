// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-01-12 15:38
// ********************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FS.Reflection
{
    /// <summary>
    ///     使用表达式树的方式创建实例
    /// </summary>
    public class Activator
    {
        /// <summary>
        ///     要反射的对象类型缓存
        /// </summary>
        private static readonly ConcurrentDictionary<int, Func<object[], object>> CacheList = new();

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <param name="type"> 类型 </param>
        /// <param name="args"> 构造函数的参数列表 </param>
        public static T CreateInstance<T>(Type type, params object[] args) where T : class
        {
            //根据参数列表返回参数类型数组
            var parameterTypes = args.Select(selector: c => c.GetType()).ToArray();

            // 生成字典Key
            var hashCode                     = type.GetHashCode();
            foreach (var o in args) hashCode += o.GetHashCode();

            Func<object[], object> val;
            if (!CacheList.TryGetValue(key: hashCode, value: out val))
            {
                val = CreateInstance(type: type, parameterTypes: parameterTypes);
                CacheList.TryAdd(key: hashCode, value: val);
            }

            return (T)val(arg: args);
        }

        /// <summary>
        ///     创建用来返回构造函数的委托
        /// </summary>
        /// <param name="type"> 对象类型 </param>
        /// <param name="parameterTypes"> 构造函数的参数类型数组 </param>
        private static Func<object[], object> CreateInstance(Type type, params Type[] parameterTypes)
        {
            //根据参数类型数组来获取构造函数
            var constructor = parameterTypes == null ? type.GetTypeInfo().GetConstructor(types: new Type[0]) : type.GetTypeInfo().GetConstructor(types: parameterTypes);
            Check.NotNull(value: constructor, parameterName: type.FullName + "：不存在该参数个数签名的构造函数");

            //构造函数参数值
            var lambdaParam = Expression.Parameter(type: typeof(object[]), name: "_args");

            //创建构造函数的参数表达式数组
            var constructorParam = parameterTypes == null ? null : BuildParameters(paramExp: lambdaParam, parameterTypes: parameterTypes);

            //创建构造函数表达式
            var newExp = Expression.New(constructor: constructor, arguments: constructorParam);

            //创建lambda表达式，返回构造函数
            var lambdaExp = Expression.Lambda<Func<object[], object>>(body: newExp, lambdaParam);

            return lambdaExp.Compile();
        }

        /// <summary>
        ///     根据类型数组和lambda表达式的参数，转化参数成相应类型
        /// </summary>
        /// <param name="parameterTypes"> 类型数组 </param>
        /// <param name="paramExp"> lambda表达式的参数表达式（参数是：object[]） </param>
        /// <returns> 构造函数的参数表达式数组 </returns>
        private static Expression[] BuildParameters(ParameterExpression paramExp, params Type[] parameterTypes)
        {
            var list = new List<Expression>();
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                //从参数表达式（参数是：object[]）中取出参数
                var arg = Expression.ArrayIndex(array: paramExp, index: Expression.Constant(value: i));
                //把参数转化成指定类型
                var argCast = Expression.Convert(expression: arg, type: parameterTypes[i]);

                list.Add(item: argCast);
            }

            return list.ToArray();
        }
    }
}