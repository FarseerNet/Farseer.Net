using System;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class Extend
    {
        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="defType">要转换的对象的类型</param>
        public static object ConvertType(this object sourceValue, Type defType)
        {
            return ConvertHelper.ConvertType(sourceValue, defType);
        }


        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue">要转换的源对象</param>
        /// <param name="defValue">转换失败时，代替的默认值</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static T ConvertType<T>(this object sourceValue, T defValue = default(T))
        {
            return ConvertHelper.ConvertType(sourceValue, defValue);
        }
    }
}