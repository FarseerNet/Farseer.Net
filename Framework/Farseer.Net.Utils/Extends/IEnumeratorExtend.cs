using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerator<int> lst, int? value)
        {
            return lst.Contains(value.GetValueOrDefault());
        }
    }
}