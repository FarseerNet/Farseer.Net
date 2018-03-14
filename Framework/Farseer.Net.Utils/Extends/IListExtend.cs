using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FS.Cache;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     获取List最后一项
        /// </summary>
        /// <typeparam name="T">任何对象</typeparam>
        /// <param name="lst">List列表</param>
        public static T GetLast<T>(this IList<T> lst)
        {
            return lst.Count > 0 ? lst[lst.Count - 1] : default(T);
        }

        /// <summary>
        ///     生成测试数据
        /// </summary>
        /// <typeparam name="TEntity">实体</typeparam>
        /// <param name="lst">列表</param>
        /// <param name="count">生成的数据</param>
        public static List<TEntity> TestData<TEntity>(this IList<TEntity> lst, int count) where TEntity : class, new()
        {
            lst = new List<TEntity>();
            for (var i = 0; i < count; i++) { lst.Add(new TEntity().FillRandData()); }
            return lst.ToList();
        }

        /// <summary>
        ///     清除重复的词语（每项中的每个字符对比）
        ///     然后向右横移一位，按最长到最短截取匹配每一项
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<string> ClearRepeat(this IList<string> lst)
        {
            for (var index = 0; index < lst.Count; index++) // 迭代所有关键词
            {
                var key = lst[index];
                for (var moveIndex = 0; moveIndex < key.Length; moveIndex += 1) // 每次移动后2位当前关键词
                {
                    for (var step = key.Length; (step - moveIndex) >= 2; step--) // 每次减少1位来对比
                    {
                        var clearKey = key.Substring(moveIndex, step - moveIndex); // 截取的关键词

                        for (var index2 = index + 1; index2 < lst.Count; index2++) // 清除下一项的所有字符串
                        { lst[index2] = lst[index2].Replace(clearKey, "").Trim(); }
                    }
                }
            }

            for (var i = 0; i < lst.Count; i++)
            {
                if (lst[i].IsNullOrEmpty())
                {
                    lst.RemoveAt(i);
                    i--;
                }
            }
            return lst.ToList();
        }

        /// <summary>
        ///     自动填充到指定数量
        /// </summary>
        public static IList Fill(this IList lst, int maxCount, object defValue)
        {
            while (true)
            {
                if (lst.Count >= maxCount) { break; }
                lst.Add(defValue);
            }

            return lst;
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public static DataTable ToTable(this IList list)
        {
            var result = new DataTable();
            if (list.Count <= 0) { return result; }

            var propertys = list[0].GetType().GetProperties();
            foreach (var pi in propertys) { result.Columns.Add(pi.Name, pi.PropertyType); }

            foreach (var entity in list)
            {
                var tempList = new ArrayList();
                foreach (var obj in propertys.Select(pi => PropertyGetCacheManger.Cache(pi, entity))) { tempList.Add(obj); }
                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
            return result;
        }

        /// <summary>
        ///     将泛型集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToTable(this IList list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            var result = new DataTable();
            if (list.Count <= 0) { return result; }
            var propertys = list[0].GetType().GetProperties();
            foreach (var pi in propertys)
            {
                if (propertyNameList.Count == 0) { result.Columns.Add(pi.Name, pi.PropertyType); }
                else
                { if (propertyNameList.Contains(pi.Name)) { result.Columns.Add(pi.Name, pi.PropertyType); } }
            }

            foreach (var entity in list)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        //var obj = pi.GetValue(info, null);
                        var obj = PropertyGetCacheManger.Cache(pi, entity);
                        tempList.Add(obj);
                    }
                    else
                    {
                        if (!propertyNameList.Contains(pi.Name)) continue;
                        //var obj = pi.GetValue(entity, null);
                        var obj = PropertyGetCacheManger.Cache(pi, entity);
                        tempList.Add(obj);
                    }
                }
                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
            return result;
        }

        ///// <summary>
        /////     关联两个实体
        ///// </summary>
        ///// <typeparam name="T1">主实体</typeparam>
        ///// <typeparam name="T2">要附加关联的实体</typeparam>
        ///// <param name="lst">主列表</param>
        ///// <param name="JoinModule">要关联的子实体</param>
        ///// <param name="JoinModuleSelect">要附加关联的子实体的字段筛选</param>
        ///// <param name="JoinModuleID">主表关系字段</param>
        ///// <param name="defJoinModule">为空时如何处理？</param>
        ///// <param name="db">事务</param>
        //public static List<T1> Join<T1, T2>(this List<T1> lst, Expression<Func<T1, T2>> JoinModule,
        //                                    Func<T1, int?> JoinModuleID = null,
        //                                    Expression<Func<T2, object>> JoinModuleSelect = null,
        //                                    T2 defJoinModule = null, DbExecutor db = null)
        //    where T1 : IEntity, new()
        //    where T2 : IEntity, new()
        //{
        //    if (lst == null || lst.Count == 0) { return lst; }
        //    if (JoinModuleID == null) { JoinModuleID = o => o.ID; }

        //    #region 获取实际类型

        //    var memberExpression = JoinModule.Body as MemberExpression;
        //    // 获取属性类型
        //    var propertyType = (PropertyInfo)memberExpression.Member;

        //    var lstPropery = new List<PropertyInfo>();
        //    while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
        //    {
        //        memberExpression = memberExpression.Expression as MemberExpression;
        //        lstPropery.Add((PropertyInfo)memberExpression.Member);
        //    }
        //    lstPropery.Reverse();

        //    #endregion

        //    // 内容ID
        //    var lstIDs = lst.Select(JoinModuleID).ToList().Select(o => o.GetValueOrDefault()).ToList();
        //    // 详细资料
        //    var lstSub = (new T2()) is BaseCacheModel<T2>
        //                          ? BaseCacheModel<T2>.Cache(db).ToList(lstIDs)
        //                          : BaseModel<T2>.Data.Where(o => lstIDs.Contains(o.ID))
        //                                         .Select(JoinModuleSelect)
        //                                         .Select(o => o.ID)
        //                                         .ToList(db);

        //    foreach (var item in lst)
        //    {
        //        var subInfo = lstSub.FirstOrDefault(o => o.ID == JoinModuleID.Invoke(item)) ?? defJoinModule;

        //        object value = item;
        //        foreach (var propery in lstPropery)
        //        {
        //            value = propery.GetValue(value, null);
        //        }
        //        propertyType.SetValue(value, subInfo, null);
        //    }

        //    return lst;
        //}
    }
}