using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Data.ExpressionVisitor;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Data.Map;
using FS.Utils.Common;

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
        /// <param name="dbProvider"> 数据库提供者（不同数据库的特性） </param>
        /// <param name="expBuilder"> 表达式持久化 </param>
        /// <param name="setMap">实体类结构映射 </param>
        internal AbsSqlBuilder(AbsDbProvider dbProvider, ExpressionBuilder expBuilder, SetDataMap setMap)
        {
            DbProvider = dbProvider;
            ExpBuilder = expBuilder;
            SetMap     = setMap;
            Param      = new List<DbParameter>();
            Sql        = new StringBuilder();
        }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        protected AbsDbProvider DbProvider { get; }

        /// <summary>
        ///     表达式持久化
        /// </summary>
        internal ExpressionBuilder ExpBuilder { get; }

        /// <summary>
        /// 实体类结构映射
        /// </summary>
        public SetDataMap SetMap { get; }

        /// <summary>
        ///     数据库名称+表名的组合
        /// </summary>
        protected string DbTableName => string.IsNullOrEmpty(value: SetMap.DbName) ? DbProvider.KeywordAegis(fieldName: SetMap.TableName) : $"{DbProvider.KeywordAegis(fieldName: SetMap.DbName)}.{DbProvider.KeywordAegis(fieldName: SetMap.TableName)}";

        /// <summary>
        ///     Where条件表达式树的解析
        /// </summary>
        protected WhereVisitor WhereVisitor => new(dbProvider: DbProvider, map: ExpBuilder.SetMap, paramList: Param);

        /// <summary>
        ///     提供字段赋值时表达式树的解析
        /// </summary>
        private AssignVisitor AssignVisitor => new(dbProvider: DbProvider, map: ExpBuilder.SetMap, paramList: Param);

        /// <summary>
        ///     提供字段排序时表达式树的解析
        /// </summary>
        protected OrderByVisitor OrderByVisitor => new(dbProvider: DbProvider, map: ExpBuilder.SetMap, paramList: Param);

        /// <summary>
        ///     提供ExpressionBinary表达式树的解析
        /// </summary>
        protected SelectVisitor SelectVisitor => new(dbProvider: DbProvider, map: ExpBuilder.SetMap, paramList: Param);

        /// <summary>
        ///     提供字段插入表达式树的解析
        /// </summary>
        private InsertVisitor InsertVisitor => new(dbProvider: DbProvider, map: ExpBuilder.SetMap, paramList: Param);

        /// <summary>
        ///     当前生成的SQL语句
        /// </summary>
        public StringBuilder Sql { get; private set; }

        /// <summary>
        ///     当前生成的参数
        /// </summary>
        public List<DbParameter> Param { get; }

        /// <summary>
        ///     查询单条记录
        /// </summary>
        public virtual ISqlParam ToEntity()
        {
            var strSelectSql  = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            Sql.Append(value: $"SELECT TOP 1 {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}");
            return this;
        }

        /// <summary>
        ///     查询多条记录
        /// </summary>
        /// <param name="top"> 限制显示的数量 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        /// <param name="isRand"> 返回当前条件下随机的数据 </param>
        public virtual ISqlParam ToList(int top = 0, bool isDistinct = false, bool isRand = false)
        {
            var strSelectSql   = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql    = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql  = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);
            var strTopSql      = top > 0 ? $"TOP {top} " : string.Empty;
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;
            var randField      = ",NEWID() as newid";

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            if (!isRand)
                Sql.Append(value: $"SELECT {strDistinctSql}{strTopSql}{strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}");
            else if (!isDistinct && string.IsNullOrWhiteSpace(value: strOrderBySql))
                Sql.Append(value: $"SELECT {strTopSql}{strSelectSql}{randField} FROM {DbTableName} {strWhereSql} ORDER BY NEWID()");
            else
                Sql.Append(value: $"SELECT {strTopSql} *{randField} FROM (SELECT {strDistinctSql} {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}) s ORDER BY NEWID()");

            return this;
        }

        /// <summary>
        ///     查询多条记录
        /// </summary>
        /// <param name="pageSize"> 每页显示数量 </param>
        /// <param name="pageIndex"> 分页索引 </param>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        public virtual ISqlParam ToList(int pageSize, int pageIndex, bool isDistinct = false)
        {
            // 不分页
            if (pageIndex == 1)
            {
                ToList(top: pageSize, isDistinct: isDistinct);
                return this;
            }

            var strSelectSql   = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql    = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql  = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            Check.IsTure(isTrue: string.IsNullOrWhiteSpace(value: strOrderBySql) && ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Count == 0, parameterName: "不指定排序字段时，需要设置主键ID");

            strOrderBySql = "ORDER BY " + (string.IsNullOrWhiteSpace(value: strOrderBySql) ? $"{IEnumerableHelper.ToString(lst: ExpBuilder.SetMap.PhysicsMap.PrimaryFields.Select(selector: o => o.Value.Name))} ASC" : strOrderBySql);
            var strOrderBySqlReverse = strOrderBySql.Replace(oldValue: " DESC", newValue: " [倒序]").Replace(oldValue: "ASC", newValue: "DESC").Replace(oldValue: "[倒序]", newValue: "ASC");

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: string.Format(format: "SELECT {0}TOP {2} {1} FROM (SELECT TOP {3} * FROM {4} {5} {6}) a  {7}", strDistinctSql, strSelectSql, pageSize, pageSize * pageIndex, DbTableName, strWhereSql, strOrderBySql, strOrderBySqlReverse));
            return this;
        }

        /// <summary>
        ///     插入
        /// </summary>
        public virtual ISqlParam Insert()
        {
            var insertAssemble = InsertVisitor.Visit(exp: ExpBuilder.ExpAssign);
            Sql.Append(value: $"INSERT INTO {DbTableName} {insertAssemble}");
            return this;
        }

        /// <summary>
        ///     批量插入
        /// </summary>
        public virtual ISqlParam InsertBatch<TEntity>(IEnumerable<TEntity> lst) where TEntity : class, new()
        {
            throw new Exception("未实现");
            // foreach (var entity in lst)
            // {
            //     ExpBuilder.AssignInsert(entity: entity);
            //     var insertAssemble = InsertVisitor.Visit(exp: ExpBuilder.ExpAssign);
            //     Sql.Append(value: $"INSERT INTO {DbTableName} {insertAssemble};");
            //     InsertVisitor.Clear();
            // }
            // return this;
        }

        /// <summary>
        ///     插入，并返回标识
        /// </summary>
        public virtual ISqlParam InsertIdentity()
        {
            var insertAssemble = InsertVisitor.Visit(exp: ExpBuilder.ExpAssign);
            Sql.Append(value: $"INSERT INTO {DbTableName} {insertAssemble}");
            return this;
        }

        /// <summary>
        ///     修改
        /// </summary>
        public virtual ISqlParam Update()
        {
            var strAssemble = AssignVisitor.Visit(exp: ExpBuilder.ExpAssign);
            var strWhereSql = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);

            // 主键如果有值、或者设置成只读条件，则自动转成条件
            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"UPDATE {DbTableName} SET {strAssemble} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询数量
        /// </summary>
        /// <param name="isDistinct"> 返回当前条件下非重复数据 </param>
        public virtual ISqlParam Count(bool isDistinct = false)
        {
            var strWhereSql    = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strDistinctSql = isDistinct ? "Distinct " : string.Empty;

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"SELECT {strDistinctSql}Count(0) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询单个值
        /// </summary>
        public virtual ISqlParam GetValue()
        {
            var strSelectSql  = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql   = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);
            var strOrderBySql = OrderByVisitor.Visit(lstExp: ExpBuilder.ExpOrderBy);

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            if (!string.IsNullOrWhiteSpace(value: strOrderBySql)) strOrderBySql = "ORDER BY " + strOrderBySql;

            Sql.Append(value: $"SELECT TOP 1 {strSelectSql} FROM {DbTableName} {strWhereSql} {strOrderBySql}");
            return this;
        }

        /// <summary>
        ///     删除
        /// </summary>
        public virtual ISqlParam Delete()
        {
            var strWhereSql = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"DELETE FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     添加或者减少某个字段
        /// </summary>
        public virtual ISqlParam AddUp()
        {
            Check.IsTure(isTrue: ExpBuilder.ExpAssign == null, parameterName: "赋值的参数不能为空！");

            var strAssemble = AssignVisitor.Visit(exp: ExpBuilder.ExpAssign);
            var strWhereSql = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"UPDATE {DbTableName} SET {strAssemble} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     累计和
        /// </summary>
        public virtual ISqlParam Sum()
        {
            var strSelectSql = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql  = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(value: strSelectSql)) strSelectSql = "0";

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"SELECT SUM({strSelectSql}) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询最大数
        /// </summary>
        public virtual ISqlParam Max()
        {
            var strSelectSql = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql  = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(value: strSelectSql)) strSelectSql = "0";

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"SELECT MAX({strSelectSql}) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        /// <summary>
        ///     查询最小数
        /// </summary>
        public virtual ISqlParam Min()
        {
            var strSelectSql = SelectVisitor.Visit(exp: ExpBuilder.ExpSelect);
            var strWhereSql  = WhereVisitor.Visit(exp: ExpBuilder.ExpWhere);

            if (string.IsNullOrWhiteSpace(value: strSelectSql)) strSelectSql = "0";

            if (!string.IsNullOrWhiteSpace(value: strWhereSql)) strWhereSql = "WHERE " + strWhereSql;

            Sql.Append(value: $"SELECT MIN({strSelectSql}) FROM {DbTableName} {strWhereSql}");
            return this;
        }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing"> 是否释放托管资源 </param>
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
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }
}