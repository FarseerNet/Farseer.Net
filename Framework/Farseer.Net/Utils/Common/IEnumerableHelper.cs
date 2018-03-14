// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-08-17 11:04
// ********************************************

using System;
using System.Collections;
using System.Text;

namespace FS.Utils.Common
{
    // ReSharper disable once InconsistentNaming
    public class IEnumerableHelper
    {
        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst">要拼接的LIST</param>
        /// <param name="sign">分隔符</param>
        public static string ToString(IEnumerable lst, string sign = ",")
        {
            if (lst == null) { return String.Empty; }
            var sb = new StringBuilder();
            foreach (var item in lst) { sb.Append(item + sign); }
            return sb.Length > 0 ? sb.Remove(sb.Length - sign.Length, sign.Length).ToString() : String.Empty;
        }
    }
}