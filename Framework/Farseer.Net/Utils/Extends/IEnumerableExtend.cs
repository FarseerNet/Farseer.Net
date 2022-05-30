using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Collections.Pooled;
using FS.Utils.Common;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class Extend
    {
        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst"> 数据源 </param>
        /// <param name="value"> 要判断的值 </param>
        /// <returns> </returns>
        public static bool Contains(this IEnumerable<int> lst, int? value) => Enumerable.Contains(source: lst, value: value.GetValueOrDefault());

        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst"> 数据源 </param>
        /// <param name="value"> 要判断的值 </param>
        /// <returns> </returns>
        public static bool Contains(this IEnumerable<long> lst, long? value) => Enumerable.Contains(source: lst, value: value.GetValueOrDefault());

        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst"> 要拼接的LIST </param>
        /// <param name="sign"> 分隔符 </param>
        public static string ToString(this IEnumerable lst, string sign = ",") => IEnumerableHelper.ToString(lst: lst, sign: sign);

        /// <summary>
        ///     对List，按splitCount大小进行分组
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="splitCount"> 每组大小 </param>
        public static IEnumerable<IEnumerable<TEntity>> Split<TEntity>(this IEnumerable<TEntity> lst, int splitCount)
        {
            if (lst == null) yield break;

            var groupLength = (int)Math.Ceiling(d: (decimal)lst.Count()                                                 / splitCount);
            for (var pageIndex = 0; pageIndex < groupLength; pageIndex++)
            {
                yield return lst.Skip(count: splitCount * pageIndex).Take(count: splitCount);
            }
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        public static List<TEntity> Clone<TEntity>(this IEnumerable<TEntity> source)
        {
            if (source == null) return null;
            if (!source.Any()) return new List<TEntity>();

            return JsonConvert.DeserializeObject<List<TEntity>>(JsonConvert.SerializeObject(source), new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            // IFormatter formatter = new BinaryFormatter();
            // Stream     stream    = new MemoryStream();
            // using (stream)
            // {
            //     formatter.Serialize(stream, source);
            //     stream.Seek(0, SeekOrigin.Begin);
            //     return (List<TEntity>)formatter.Deserialize(stream);
            // }
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="pageSize"> 每页大小 </param>
        /// <param name="pageIndex"> 索引 </param>
        public static PooledList<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, int pageSize, int pageIndex = 1)
        {
            if (pageSize == 0) return lst.ToPooledList();

            #region 计算总页数

            var allCurrentPage = 0;
            var recordCount    = lst.Count();
            if (pageIndex < 1)
            {
                return lst.Take(count: pageSize).ToPooledList();
            }

            if (pageSize < 1) pageSize = 10;

            if (pageSize != 0)
            {
                allCurrentPage = recordCount / pageSize;
                allCurrentPage = recordCount % pageSize != 0 ? allCurrentPage + 1 : allCurrentPage;
                allCurrentPage = allCurrentPage         == 0 ? 1 : allCurrentPage;
            }

            if (pageIndex > allCurrentPage) pageIndex = allCurrentPage;

            #endregion

            return lst.Skip(count: pageSize * (pageIndex - 1)).Take(count: pageSize).ToPooledList();
        }

        /// <summary>
        ///     对List进行分页
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="lst"> List列表 </param>
        /// <param name="pageSize"> 每页大小 </param>
        /// <param name="pageIndex"> 索引 </param>
        /// <param name="recordCount"> 总数量 </param>
        public static PooledList<TEntity> ToList<TEntity>(this IEnumerable<TEntity> lst, int pageSize, int pageIndex, out int recordCount)
        {
            recordCount = lst.Count();
            return ToList(lst: lst, pageSize: pageSize, pageIndex: pageIndex);
        }

        /// <summary>
        ///     字段选择器
        /// </summary>
        /// <param name="select"> 字段选择器 </param>
        /// <param name="lst"> 列表 </param>
        public static List<T> ToSelectList<TEntity, T>(this IEnumerable<TEntity> lst, Func<TEntity, T> select) => lst?.Select(selector: select).ToList();
    }
}