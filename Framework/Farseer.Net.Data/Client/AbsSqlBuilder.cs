using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Data.ExpressionVisitor;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Utils.Common;
using Newtonsoft.Json;

namespace FS.Data.Client
{
    /// <summary>
    ///     通用SQL生成器
    /// </summary>
    public abstract class AbsSqlBuilder : ISqlParam
    {
        /// <summary>
        ///     查询支持的SQL方法
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        /// <param name="dbName">数据库名称</param>
        internal AbsSqlBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder,string dbName, string tableName)
        {
            DbProvider = dbProvider;
            ExpBuilder = expBuilder;
            DbName = dbName;
            TableName = tableName;
            Param = new List<DbParameter>();
            Sql = new StringBuilder();
        }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        protected AbsDbProvider DbProvider { get; }

        /// <summary>
        ///     当前生成的SQL语句
        /// </summary>
        public StringBuilder Sql { get; private set; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        public List<DbParameter> Param { get; }

        /// <summary>
        ///     表达式持久化
        /// </summary>
        internal ExpressionBuilder ExpBuilder { get; }

        /// <summary>
        ///     数据库名称
        /// </summary>
        public string DbName { get; }

        /// <summary>
        ///     表名/视图名/存储过程名
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// 数据库名称+表名的组合
        /// </summary>
        protected string DbTableName => string.IsNullOrEmpty(DbName) ? DbProvider.KeywordAegis(TableName) : $"{DbProvider.KeywordAegis(DbName)}.{DbProvider.KeywordAegis(TableName)}";

        /// <summary>
        ///     Where条件表达式树的解析
        /// </summary>
        protected WhereVisitor WhereVisitor => new(DbProvider, ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供字段赋值时表达式树的解析
        /// </summary>
        private AssignVisitor AssignVisitor => new (DbProvider, ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供字段排序时表达式树的解析
        /// </summary>
        protected OrderByVisitor OrderByVisitor => new (DbProvider, ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        protected SelectVisitor SelectVisitor => new (DbProvider, ExpBuilder.SetMap, Param);

        /// <summary>
        ///     提供字段插入表达式树的解析
        /// </summary>
        private InsertVisitor InsertVisitor => new (DbProvider, ExpBuilder.SetMap, Param);

        /// <summary>
        ///     查询单条记录
        /// </summary>
        public virtual ISqlParam ToEntity()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT TOP 1 {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}");
            return this;
        }

        /// <summary>
        ///     查询多条记录
        /// </summary>
        /// <param name="top">限制显示的数量</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        /// <param name="isRand">返回当前条件下随机的数据</param>
        public virtual ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);
            var strTopSql = top > 0 ? $"TOP {top} " : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField = ",NEWID() as newid";

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            if (!isRand) { Sql.Append($"SELECT {strDistinctSql}{strTopSql}{strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}"); }
            else if (!isDistinct && string.IsNullOrWhiteSpace(strOrderBySql))
            {
                Sql.Append($"SELECT {strTopSql}{strSelectSql}{randField} FROM {DbTableName} {strWhereSql} ORDER BY NEWID()");
            }
            else
            {
                Sql.Append($"SELECT {strTopSql} *{randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}) s ORDER BY NEWID()");
            }
            return this;
        }

        /// <summary>
        ///     查询多条记录
        /// </summary>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public virtual ISqlParam ToList(int pageSize, int pageIndex, bool isDistinct = false)
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

            Check.IsTure(string.IsNullOrWhiteSpace(strOrderBySql) && ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Count == 0, "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(strOrderBySql) ? $"{IEnumerableHelper.ToString(ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Select(o => o.Value.Name))} ASC" : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append(string.Format("SELECT {0}TOP {2} {1} FROM (SELECT TOP {3} * FROM {4} {5} {6}) a  {7}", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, DbTableName, strWhereSql, strOrderBySql, strOrderBySqlReverse));
            return this;
        }

        /// <summary>
        ///     插入
        /// </summary>
        public virtual ISqlParam Insert()
        {
            var insertAssemble = InsertVisitor.Visit(ExpBuilder.ExpAssign);
            Sql.Append($"INSERT INTO {DbTableName} {insertAssemble}");
            return this;
        }

        /// <summary>
        ///     批量插入
        /// </summary>
        public virtual ISqlParam InsertBatch<TEntity>(IEnumerable<TEntity> lst) where TEntity : class, new()
        {
            foreach (var entity in lst)
            {
                ExpBuilder.AssignInsert(entity);
                var insertAssemble = InsertVisitor.Visit(ExpBuilder.ExpAssign);
                Sql.Append($"INSERT INTO {DbTableName} {insertAssemble};");
                InsertVisitor.Clear();
            }
            return this;
        }

        /// <summary>
        ///     插入，并返回标识
        /// </summary>
        public virtual ISqlParam InsertIdentity()
        {
            var insertAssemble = InsertVisitor.Visit(ExpBuilder.ExpAssign);
            Sql.Append($"INSERT INTO {DbTableName} {insertAssemble}");
            return this;
        }

        /// <summary>
        ///     修改
        /// </summary>
        public virtual ISqlParam Update()
        {
            var strAssemble = AssignVisitor.Visit(ExpBuilder.ExpAssign);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            // 主键如果有值、或者设置成只读条件，则自动转成条件
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"UPDATE {DbTableName} SET {strAssemble} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询数量
        /// </summary>
        /// <param name="isDistinct">返回当前条件下非重复数据</param>
        public virtual ISqlParam Count(bool isDistinct = false)
        {
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT {strDistinctSql}Count(0) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询单个值
        /// </summary>
        public virtual ISqlParam GetValue()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }
            if (!string.IsNullOrWhiteSpace(strOrderBySql)) { strOrderBySql = "ORDER BY " + strOrderBySql; }

            Sql.Append($"SELECT TOP 1 {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}");
            return this;
        }

        /// <summary>
        ///     删除
        /// </summary>
        public virtual ISqlParam Delete()
        {
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"DELETE FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     添加或者减少某个字段
        /// </summary>
        public virtual ISqlParam AddUp()
        {
            Check.IsTure(ExpBuilder.ExpAssign == null, "赋值的参数不能为空！");

            var strAssemble = AssignVisitor.Visit(ExpBuilder.ExpAssign);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"UPDATE {DbTableName} SET {strAssemble} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     累计和
        /// </summary>
        public virtual ISqlParam Sum()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT SUM({strSelectSql}) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询最大数
        /// </summary>
        public virtual ISqlParam Max()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT MAX({strSelectSql}) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询最小数
        /// </summary>
        public virtual ISqlParam Min()
        {
            var strSelectSql = SelectVisitor.Visit(ExpBuilder.ExpSelect);
            var strWhereSql = WhereVisitor.Visit(ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(strSelectSql)) { strSelectSql = "0"; }
            if (!string.IsNullOrWhiteSpace(strWhereSql)) { strWhereSql = "WHERE " + strWhereSql; }

            Sql.Append($"SELECT MIN({strSelectSql}) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                Sql.Clear();
                Sql = null;
                Param?.Clear();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}