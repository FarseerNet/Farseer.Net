﻿using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Data.Map;

namespace FS.Data.Client.PostgreSql
{
    /// <summary>
    ///     针对MySql 数据库 SQL生成器
    /// </summary>
    public class PostgreSqlBuilder : AbsSqlBuilder
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider"> 数据库提供者（不同数据库的特性） </param>
        /// <param name="expBuilder"> 表达式持久化 </param>
        /// <param name="setMap">实体类结构映射 </param>
        internal PostgreSqlBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, SetDataMap setMap) : base(dbProvider: dbProvider, expBuilder: expBuilder, setMap)
        {
        }

        public override ISqlParam ToEntity()
        {
            var strSelectSql  = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql     = "WHERE "    + strWhereSql;
            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            Sql.Append(value: $"SELECT {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql} LIMIT 1");
            return this;
        }

        public override ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql  = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);

            var strTopSql      = top > 0 ? $"LIMIT {top}" : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField      = ",random() as newid";

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql     = "WHERE "    + strWhereSql;
            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            if (!isRand)
                Sql.Append(value: $"SELECT {strDistinctSql}{strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql} {strTopSql}");
            else if (!isDistinct && string.IsNullOrWhiteSpace(value: strOrderBySql))
                Sql.Append(value: $"SELECT {strTopSql}{strSelectSql}{randField} FROM {DbTableName} {strWhereSql} ORDER BY random()");
            else
                Sql.Append(value: $"SELECT {strTopSql} *{randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}) s ORDER BY random()");
            return this;
        }

        public override ISqlParam ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1)
            {
                ToList(top: pageSize, isDistinct: isDistinct);
                return this;
            }

            var strSelectSql  = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);

            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql     = "WHERE "    + strWhereSql;
            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            Sql.Append(value: $"SELECT {strDistinctSql}{strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql} LIMIT {pageSize} OFFSET {pageSize * (pageIndex - 1)}");
            return this;
        }

        public override ISqlParam GetValue()
        {
            var strSelectSql  = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql     = "WHERE "    + strWhereSql;
            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            Sql.Append(value: $"SELECT {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql} LIMIT 1");
            return this;
        }

        public override ISqlParam InsertIdentity()
        {
            base.InsertIdentity();
            Sql.Append(value: $" \r\nRETURNING {DbProvider.KeywordAegis(fieldName: ExpBuilder.SetMap.PhysicsMap.DbGeneratedFields.Value.Name)}");
            return this;
        }
    }
}