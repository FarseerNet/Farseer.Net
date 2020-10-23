using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.Oracle
{
    /// <summary>
    ///     针对Oracle 数据库 SQL生成器
    /// </summary>
    public class OracleBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        /// <param name="dbName">数据库名称 </param>
        internal OracleBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder,string dbName, string tableName) : base(dbProvider, expBuilder,dbName, tableName)
        {
        }

        public override ISqlParam ToEntity()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append(BuilderTop(1, $"SELECT {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}"));
            return this;
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField = ",dbms_random.value as newid";

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) Sql.Append(BuilderTop(top, $"SELECT {strDistinctSql}{strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}"));
            else if (!isDistinct && string.IsNullOrWhiteSpace(strOrderBySql)) Sql.Append(BuilderTop(top, $"SELECT {strSelectSql}{randField} FROM {DbTableName} {strWhereSql} ORDER BY dbms_random.value"));
            else Sql.Append(BuilderTop(top, $"SELECT * {randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}) s ORDER BY dbms_random.value"));
            return this;
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

            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append(string.Format("SELECT * FROM ( SELECT A.*, ROWNUM RN FROM (SELECT {0}{1} FROM {4} {5} {6}) A WHERE ROWNUM <= {3} ) WHERE RN > {2}", strDistinctSql, strSelectSql, pageSize * (pageIndex - 1), pageSize * pageIndex, DbTableName, strWhereSql, strOrderBySql));
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }
            var strTopSql = (string.IsNullOrWhiteSpace(strWhereSql) ? "WHERE" : "AND") + " rownum <=1";

            Sql.Append($"SELECT {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql} {strTopSql}");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append(";SELECT @@IDENTITY ");
            return this;
        }

        private string BuilderTop(int top, string sql)
        {
            if (top > 0) return $"SELECT * FROM ({sql}) WHERE rownum <={top}";
            return sql;
        }
    }
}