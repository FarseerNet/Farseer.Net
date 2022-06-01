// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-11-08 14:21
// ********************************************

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using Newtonsoft.Json;

namespace FS.Core
{
    /// <summary>
    ///     Json帮助类
    /// </summary>
    public static class Jsons
    {
        /// <summary>
        ///     转换到对象（失败时，用defVal代替）
        /// </summary>
        /// <typeparam name="T"> 要转换的对象 </typeparam>
        /// <param name="obj"> json字符串 </param>
        /// <param name="defVal"> 失败时，用defVal代替 </param>
        public static T ToObject<T>(object obj, T defVal)
        {
            if (obj == null) return defVal;
            var json = obj.ToString();
            if (string.IsNullOrWhiteSpace(value: json)) return defVal;
            try
            {
                return JsonConvert.DeserializeObject<T>(value: json);
            }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        ///     转换到对象（失败时，用defVal代替）
        /// </summary>
        /// <typeparam name="T"> 要转换的对象 </typeparam>
        /// <param name="obj"> json字符串 </param>
        public static T ToObject<T>(object obj) => ToObject(obj: obj, defVal: default(T));

        /// <summary>
        ///     转换到对象（失败时，用defVal代替）
        /// </summary>
        /// <typeparam name="T"> 要转换的对象 </typeparam>
        /// <param name="jsons"> json字符串 </param>
        public static PooledList<T> ToObject<T>(IEnumerable<string> jsons)
        {
            return jsons.Select(selector: ToObject<T>).Where(predicate: adminOprLogVO => adminOprLogVO != null).ToPooledList();
        }
    }
}