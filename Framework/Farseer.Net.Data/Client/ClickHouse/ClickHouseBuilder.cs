using System.Collections.Generic;
using System.Linq;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Data.Map;
using Newtonsoft.Json;

namespace FS.Data.Client.ClickHouse
{
    /// <summary>
    ///     针对MySql 数据库 SQL生成器
    /// </summary>
    public class ClickHouseBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        /// <param name="dbName">数据库名称 </param>
        internal ClickHouseBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, string dbName, string tableName) : base(dbProvider, expBuilder, dbName, tableName)
        {
        }

        public override ISqlParam ToEntity()
        {
            var strSelectSql  = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                strOrderBySql = "ORDER BY " + strOrderBySql;
            }

            Sql.Append($"SELECT {strSelectSql} FROM {DbTableName} final {strWhereSql} {strOrderBySql} LIMIT 1");
            return this;
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql  = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strTopSql      = top > 0 ? $"LIMIT {top}" : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField      = ",Rand() as newid";

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                strOrderBySql = "ORDER BY " + strOrderBySql;
            }

            if (!isRand)
            {
                Sql.Append($"SELECT {strDistinctSql}{strSelectSql} FROM {DbTableName} final {strWhereSql} {strOrderBySql} {strTopSql}");
            }
            else if (!isDistinct && string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Sql.Append($"SELECT {strSelectSql}{randField} FROM {DbTableName} final {strWhereSql} ORDER BY Rand() {strTopSql}");
            }
            else
            {
                Sql.Append($"SELECT * {randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbTableName} final {strWhereSql} {strOrderBySql}) s ORDER BY Rand() {strTopSql}");
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

            var strSelectSql  = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                strOrderBySql = "ORDER BY " + strOrderBySql;
            }

            Sql.Append($"SELECT {strDistinctSql}{strSelectSql} FROM {DbTableName} final {strWhereSql} {strOrderBySql} LIMIT {pageSize * (pageIndex - 1)},{pageSize}");
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql  = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            if (!string.IsNullOrWhiteSpace(strOrderBySql))
            {
                strOrderBySql = "ORDER BY " + strOrderBySql;
            }

            Sql.Append($"SELECT {strSelectSql} FROM {DbTableName} final {strWhereSql} {strOrderBySql} LIMIT 1");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append(";SELECT @@IDENTITY;");
            return this;
        }


        /// <summary>
        ///     批量插入
        /// </summary>
        public override ISqlParam InsertBatch<TEntity>(IEnumerable<TEntity> lst)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new JsonFieldNameContractResolver(ExpBuilder.SetMap)
            };

            var sql = string.Join(",", lst.Select(o => JsonConvert.SerializeObject(o, settings)));
            Sql.Append($"INSERT INTO {DbTableName} FORMAT JSONEachRow {sql}");
            return this;
        }

        /// <summary>
        ///     查询数量
        /// </summary>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public override ISqlParam Count(bool isDistinct = false)
        {
            var strWhereSql    = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            Sql.Append($"SELECT {strDistinctSql}Count(0) FROM {DbTableName} final {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     累计和
        /// </summary>
        public override ISqlParam Sum()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql  = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql))
            {
                strSelectSql = "0";
            }

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            Sql.Append($"SELECT SUM({strSelectSql}) FROM {DbTableName} final {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询最大数
        /// </summary>
        public override ISqlParam Max()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql  = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql))
            {
                strSelectSql = "0";
            }

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            Sql.Append($"SELECT MAX({strSelectSql}) FROM {DbTableName} final {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询最小数
        /// </summary>
        public override ISqlParam Min()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql  = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql))
            {
                strSelectSql = "0";
            }

            if (!string.IsNullOrWhiteSpace(strWhereSql))
            {
                strWhereSql = "WHERE " + strWhereSql;
            }

            Sql.Append($"SELECT MIN({strSelectSql}) FROM {DbTableName} final {strWhereSql}");
            return this;
        }
    }
}