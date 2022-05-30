using System;
using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using FS.Utils.Component;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static partial class Extend
    {
        /// <summary>
        ///     获取枚举中文
        /// </summary>
        /// <param name="eum"> 枚举值 </param>
        public static string GetName(this Enum eum) => new Enums(eum: eum).GetName();

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static PooledDictionary<int, string> ToDictionary(this Type enumType)
        {
            var dic = new PooledDictionary<int, string>();
            foreach (int value in Enum.GetValues(enumType: enumType)) dic.Add(key: value, value: new Enums(eum: (Enum)Enum.ToObject(enumType: enumType, value: value)).GetName());
            return dic;
        }

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static PooledList<int> ToList(this Type enumType) => Enum.GetValues(enumType: enumType).Cast<int>().ToPooledList();

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static PooledList<TEnum> ToList<TEnum>(this Type enumType) => Enum.GetValues(enumType: enumType).Cast<TEnum>().ToPooledList();
    }
}