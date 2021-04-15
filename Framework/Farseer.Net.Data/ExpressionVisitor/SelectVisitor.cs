using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using FS.Core.Mapping;
using FS.Data.Client;
using FS.Data.Map;
using FS.Utils.Common;

namespace FS.Data.ExpressionVisitor
{
    /// <summary>
    ///     提供Select筛选字段时表达式树的解析
    /// </summary>
    public class SelectVisitor : AbsSqlVisitor
    {
        /// <summary>
        ///     Select筛选字段时表达式树的解析
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public SelectVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        public new string Visit(System.Linq.Expressions.Expression exp)
        {
            base.Visit(exp);
            var str = IEnumerableHelper.ToString(SqlList.Reverse());
            return str.Length > 0 ? str : "*";
        }

        /// <summary>
        ///     加入字段到队列中
        /// </summary>
        /// <param name="keyValue">当前字段属性</param>
        protected override void VisitMemberAccess(KeyValuePair<PropertyInfo, DbFieldMapState> keyValue)
        {
            SqlList.Push(keyValue.Value.Field.IsFun ? CurrentFieldName + " as " + keyValue.Key.Name : DbProvider.KeywordAegis(CurrentFieldName));
        }
    }
}