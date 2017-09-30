using System;
using System.Reflection;

namespace Farseer.Net
{
    /// <summary>
    ///     检测
    /// </summary>
    public static class Check
    {
        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <param name="isTrue">为true时，报异常</param>
        /// <param name="parameterName">参数名称</param>
        public static void IsTure(bool isTrue, string parameterName)
        {
            if (!isTrue) return;
            throw new FarseerException(parameterName);
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <param name="isTrue">为true时，报异常</param>
        /// <param name="parameterName">参数名称</param>
        public static void IsTure<TFarseerException>(bool isTrue, string parameterName) where TFarseerException : FarseerException
        {
            if (!isTrue) return;
            throw (TFarseerException)Activator.CreateInstance(typeof(TFarseerException), parameterName);
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <typeparam name="TValue">任何值</typeparam>
        /// <typeparam name="TFarseerException">异常类型</typeparam>
        /// <param name="value">任何值</param>
        /// <param name="parameterName">参数名称</param>
        public static TValue NotNull<TValue, TFarseerException>(TValue value, string parameterName) where TValue : class where TFarseerException : FarseerException
        {
            IsTure<TFarseerException>(value == null, parameterName);
            return value;
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <typeparam name="TValue">任何值</typeparam>
        /// <param name="value">任何值</param>
        /// <param name="parameterName">参数名称</param>
        public static TValue NotNull<TValue>(TValue value, string parameterName) where TValue : class
        {
            IsTure(value == null, parameterName);
            return value;
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <typeparam name="T">任何值</typeparam>
        /// <param name="value">任何值</param>
        public static T NotNull<T>(T value) where T : class
        {
            IsTure(value == null, typeof(T).Name);
            return value;
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <typeparam name="T">任何值</typeparam>
        /// <param name="value">任何值</param>
        /// <param name="parameterName">参数名称</param>
        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            IsTure(value == null, parameterName);
            return value;
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <param name="value">任何值</param>
        /// <param name="parameterName">参数名称</param>
        public static string NotEmpty(string value, string parameterName)
        {
            IsTure(string.IsNullOrWhiteSpace(value), parameterName);
            return value;
        }

        /// <summary>
        ///     当为空时，报参数不能为空的异常信息
        /// </summary>
        /// <param name="value">任何值</param>
        /// <param name="parameterName">参数名称</param>
        public static string NotEmpty<TFarseerException>(string value, string parameterName) where TFarseerException : FarseerException
        {
            IsTure<TFarseerException>(string.IsNullOrWhiteSpace(value), parameterName);
            return value;
        }

        /// <summary>
        ///     判断两个类型是否存在继承关系
        /// </summary>
        /// <param name="parentType">基类</param>
        /// <param name="subType">继承类</param>
        public static void AssignableFrom(Type parentType, Type subType)
        {
            if (!parentType.GetTypeInfo().IsAssignableFrom(subType)) { throw new FarseerException($"{subType.Name} 必须继承自 {parentType.Name}."); }
        }
    }
}