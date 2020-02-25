using FS.Data.Infrastructure;
using FS.Data.Internal;

namespace FS.Data.Client.OleDb
{
    /// <summary>
    ///     针对OleDb 数据库 SQL生成器
    /// </summary>
    public class OleDbBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        /// <param name="dbName">数据库名称 </param>
        internal OleDbBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder,string dbName, string tableName) : base(dbProvider, expBuilder,dbName, tableName)
        {
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strTopSql = top > 0 ? $"TOP {top}" : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField = ",Rnd(-(TestID+\" & Rnd() & \")) as newid";

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) { Sql.Append($"SELECT {strDistinctSql}{strTopSql} {strSelectSql} FROM {DbProvider.KeywordAegis(TableName)} {strWhereSql} {strOrderBySql}"); }
            else if (!isDistinct && string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Sql.Append($"SELECT {strSelectSql}{randField} FROM {DbProvider.KeywordAegis(TableName)} {strWhereSql} BY Rnd(-(TestID+\" & Rnd() & \")) {strTopSql}");
            }
            else
            {
                Sql.Append($"SELECT *{randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbProvider.KeywordAegis(TableName)} {strWhereSql} {strOrderBySql}) s BY Rnd(-(TestID+\" & Rnd() & \")) {strTopSql}");
            }
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT TOP 1 {strSelectSql} FROM {DbProvider.KeywordAegis(TableName)} {strWhereSql} {strOrderBySql}");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append(";SELECT @@IDENTITY;");
            return this;
        }
    }
}