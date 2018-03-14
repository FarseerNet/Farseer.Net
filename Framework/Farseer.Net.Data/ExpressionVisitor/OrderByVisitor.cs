using System.Collections.Generic;
using System.Data.Common;
using FS.Data.Infrastructure;
using FS.Data.Map;
using FS.Utils.Common;

namespace FS.Data.ExpressionVisitor
{
    /// <summary>
    ///     提供字段排序时表达式树的解析
    /// </summary>
    public class OrderByVisitor : AbsSqlVisitor
    {
        /// <summary>
        ///     提供字段排序时表达式树的解析
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public OrderByVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        /// <summary>
        ///     排序解析入口
        /// </summary>
        /// <param name="lstExp"></param>
        public virtual string Visit(Dictionary<System.Linq.Expressions.Expression, bool> lstExp)
        {
            if (lstExp == null) { return null; }
            var lst = new List<string>();
            foreach (var keyValue in lstExp)
            {
                Visit(keyValue.Key);
                while (SqlList.Count > 0) { lst.Add($"{SqlList.Pop()} {(keyValue.Value ? "ASC" : "DESC")}"); }
                //SqlList.Push();
            }
            lst.Reverse();
            return IEnumerableHelper.ToString(lst, ", ");
        }
    }
}