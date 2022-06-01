using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using Collections.Pooled;
using FS.Data.Client;
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
        /// <param name="dbProvider"> 数据库提供者（不同数据库的特性） </param>
        /// <param name="map"> 字段映射 </param>
        /// <param name="paramList"> SQL参数列表 </param>
        public OrderByVisitor(AbsDbProvider dbProvider, SetDataMap map, IList<DbParameter> paramList) : base(dbProvider: dbProvider, map: map, paramList: paramList)
        {
        }

        /// <summary>
        ///     排序解析入口
        /// </summary>
        /// <param name="lstExp"> </param>
        public virtual string Visit(IDictionary<Expression, bool> lstExp)
        {
            if (lstExp == null) return null;
            using var lst = new PooledList<string>();
            foreach (var keyValue in lstExp)
            {
                Visit(exp: keyValue.Key);
                while (SqlList.Count > 0) lst.Add(item: $"{SqlList.Pop()} {(keyValue.Value ? "ASC" : "DESC")}");
            }

            lst.Reverse();
            return string.Join(", ", lst);
        }
    }
}