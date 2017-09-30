using Farseer.Net.Data.Infrastructure;
using Farseer.Net.Data.Internal;

namespace Farseer.Net.Data.Client.SqLite
{
    /// <summary>
    ///     针对SqLite 数据库 SQL生成器
    /// </summary>
    public class SqLiteBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="name">表名/视图名/存储过程名</param>
        internal SqLiteBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, string name) : base(dbProvider, expBuilder, name)
        {
        }

        public override ISqlParam ToEntity()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql} LIMIT 0,1");
            return this;
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strTopSql = top > 0 ? $"LIMIT {top}" : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField = ",Rand() as newid";

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) { Sql.Append($"SELECT {strDistinctSql}{strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql} {strTopSql}"); }
            else if (!isDistinct && string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Sql.Append($"SELECT {strTopSql}{strSelectSql}{randField} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} ORDER BY Rand()");
            }
            else
            {
                Sql.Append($"SELECT {strTopSql} *{randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql}) s ORDER BY Rand()");
            }
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

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT {strDistinctSql}{strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql} LIMIT {pageSize*(pageIndex - 1)},{pageSize}");
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "*"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT {strSelectSql} FROM {DbProvider.KeywordAegis(Name)} {strWhereSql} {strOrderBySql} LIMIT 0,1");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append(";SELECT last_insert_rowid();");
            return this;
        }
    }
}