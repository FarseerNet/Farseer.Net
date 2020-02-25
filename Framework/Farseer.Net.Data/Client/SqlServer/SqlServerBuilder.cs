using System.Linq;
using System.Reflection;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Utils.Common;
using FS.Utils.Common.ExpressionVisitor;

namespace FS.Data.Client.SqlServer
{
    /// <summary>
    ///     针对SqlServer 2005+ 数据库 SQL生成器
    /// </summary>
    public class SqlServerBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        /// <param name="dbName">数据库名称 </param>
        internal SqlServerBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder,string dbName, string tableName) : base(dbProvider, expBuilder,dbName, tableName)
        {
        }

        public override ISqlParam ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1)
            {
                ToList(pageSize, isDistinct);
                return this;
            }

            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strDistinctSql = isDistinct ? "Distinct" : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Check.IsTure(string.IsNullOrWhiteSpace(strOrderBySql) && (ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Count == 0), "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? $"{IEnumerableHelper.ToString(ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Select(o => o.Value.Name), ",")} ASC" : strOrderBySql);

            Sql.Append(string.Format("SELECT {1} FROM (SELECT {0} {1},ROW_NUMBER() OVER({2}) as Row FROM {3} {4}) a WHERE Row BETWEEN {5} AND {6}", strDistinctSql, strSelectSql, strOrderBySql, DbProvider.KeywordAegis(TableName), strWhereSql, (pageIndex - 1)*pageSize + 1, pageIndex*pageSize));
            return this;
        }

        public override ISqlParam Insert()
        {
            base.Insert();

            // 主键如果有值，则需要 SET IDENTITY_INSERT ON
            if (ExpBuilder.SetMap.PhysicsMap.DbGeneratedFields.Key != null)
            {
                // 是否赋值了标识列的值
                if (new GetMemberVisitor().Visit(ExpBuilder.ExpAssign).Any(o => o.Member as PropertyInfo == ExpBuilder.SetMap.PhysicsMap.DbGeneratedFields.Key))
                {
                    var sql = Sql.ToString();
                    Sql.Clear();
                    Sql.Append(string.Format(";SET IDENTITY_INSERT {0} ON ; {1} ; SET IDENTITY_INSERT {0} OFF;", DbProvider.KeywordAegis(TableName), sql));
                }
            }
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            Insert();
            Sql.Append(";SELECT @@IDENTITY;");
            return this;
        }
    }
}