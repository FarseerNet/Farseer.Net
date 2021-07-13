using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="eum">枚举值</param>
        public static string GetName(this Enum eum) => new Enums(eum).GetName();

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static Dictionary<int, string> ToDictionary(this Type enumType)
        {
            var dic = new Dictionary<int, string>();
            foreach (int value in Enum.GetValues(enumType)) { dic.Add(value, new Enums((Enum)Enum.ToObject(enumType, value)).GetName()); }
            return dic;
        }

        /// <summary>
        ///     获取枚举列表
        /// </summary>
        public static List<int> ToList(this Type enumType)
        {
            return Enum.GetValues(enumType).Cast<int>().ToList();
        }
    }
}