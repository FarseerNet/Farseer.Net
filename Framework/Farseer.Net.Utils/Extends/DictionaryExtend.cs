using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace

namespace Farseer.Net.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     扩展 Dictionary 根据Value反向查找Key的方法
        /// </summary>
        /// <param name="list">Dictionary对象</param>
        /// <param name="t2">键值</param>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        public static T1 GetKey<T1, T2>(this Dictionary<T1, T2> list, T2 t2)
        {
            foreach (var obj in list.Where(obj => obj.Value.Equals(t2))) { return obj.Key; }
            return default(T1);
        }

        /// <summary>
        ///     转换成Json格式
        /// </summary>
        /// <param name="dic">Dictionary对象</param>
        /// <typeparam name="T1">Key</typeparam>
        /// <typeparam name="T2">Value</typeparam>
        public static string ToJson<T1, T2>(this Dictionary<T1, T2> dic)
        {
            var sb = new StringBuilder();
            foreach (var item in dic) { sb.Append(String.Format("{0}={1}&", item.Key, item.Value)); }
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
    }
}