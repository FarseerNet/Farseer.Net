using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Farseer.Net.Cache;
using Farseer.Net.Core.Mapping.Attribute;
using Farseer.Net.Data.Map;
using Farseer.Net.Utils.Common;
using Farseer.Net.Utils.Common.ExpressionVisitor;

namespace Farseer.Net.Data.Internal
{
    /// <summary> 表达式创建者 </summary>
    internal class ExpressionBuilder
    {
        internal ExpressionBuilder(SetDataMap map)
        {
            SetMap = map;
        }

        /// <summary>
        ///     排序表达式树
        /// </summary>
        internal Dictionary<Expression, bool> ExpOrderBy { get; private set; }

        /// <summary>
        ///     字段筛选表达式树
        /// </summary>
        internal Expression ExpSelect { get; private set; }

        /// <summary>
        ///     条件表达式树
        /// </summary>
        internal Expression ExpWhere { get; private set; }

        /// <summary>
        ///     赋值表达式
        /// </summary>
        internal Expression ExpAssign { get; private set; }

        /// <summary>
        ///     实体类映射
        /// </summary>
        internal SetDataMap SetMap { get; }

        /// <summary>
        ///     添加筛选
        /// </summary>
        /// <param name="select"></param>
        internal void AddSelect(Expression select)
        {
            if (select == null) return;
            ExpSelect = ExpressionHelper.MergeBlockExpression(ExpSelect, select);
        }

        /// <summary>
        ///     添加条件（主要是提供给逻辑删除使用）
        /// </summary>
        /// <param name="where">查询条件</param>
        private void AddWhere(Expression where)
        {
            if (where == null) return;
            ExpWhere = ExpressionHelper.MergeBlockExpression(ExpWhere, where);
        }

        /// <summary>
        ///     添加条件
        /// </summary>
        /// <param name="where">查询条件</param>
        internal void AddWhere<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            if (where == null) return;
            ExpWhere = ExpWhere == null ? where : ExpressionHelper.MergeAndAlsoExpression((Expression<Func<TEntity, bool>>)ExpWhere, where);
        }

        /// <summary>
        ///     添加条件
        /// </summary>
        /// <param name="where">查询条件</param>
        internal void AddWhereOr<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            if (where == null) return;
            ExpWhere = ExpWhere == null ? where : ExpressionHelper.MergeOrElseExpression((Expression<Func<TEntity, bool>>)ExpWhere, where);
        }

        /// <summary>
        ///     添加排序
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="isAsc"></param>
        internal void AddOrderBy(Expression exp, bool isAsc)
        {
            if (ExpOrderBy == null) { ExpOrderBy = new Dictionary<Expression, bool>(); }
            if (exp != null) { ExpOrderBy.Add(exp, isAsc); }
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        internal void AddAssign(Expression fieldName, object fieldValue)
        {
            if (fieldName == null) { return; }
            var u = ExpressionHelper.MergeAssignExpression(fieldName, fieldValue);
            ExpAssign = ExpressionHelper.MergeBlockExpression(ExpAssign, u);
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <param name="fieldName">字段选择器</param>
        internal void AddAssign(Expression fieldName)
        {
            if (fieldName == null) return;
            ExpAssign = ExpressionHelper.MergeBlockExpression(ExpAssign, fieldName);
        }

        /// <summary>
        ///     Insert将实体类的赋值，转成表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">被赋值的实体</param>
        internal void AssignInsert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if (entity == null) { return; }
            var oParameter = Expression.Parameter(entity.GetType(), "o");
            var lstAssign = new List<Expression>();

            //  迭代实体赋值情况
            //  这里没有限定标识字段，允许客户手动设置标识数据
            foreach (var kic in SetMap.PhysicsMap.MapList.Where(o => o.Value.Field.IsMap && !o.Value.Field.IsFun && o.Value.Field.InsertStatusType == StatusType.CanWrite))
            {
                var obj = PropertyGetCacheManger.Cache(kic.Key, entity);
                if (obj == null) { continue; }
                var member = Expression.MakeMemberAccess(oParameter, kic.Key);

                var ass = Expression.Assign(member, Expression.Convert(Expression.Constant(obj), kic.Key.PropertyType));
                lstAssign.Add(ass);
            }
            ExpAssign = ExpressionHelper.MergeBlockExpression(lstAssign.ToArray());
        }

        /// <summary>
        ///     Update将实体类的赋值，转成表达式树
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entity">被赋值的实体</param>
        internal void AssignUpdate<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if (entity == null) { return; }
            var oParameter = Expression.Parameter(entity.GetType(), "o");
            var lstAssign = new List<Expression>();

            //  迭代实体赋值情况
            foreach (var kic in SetMap.PhysicsMap.MapList.Where(o => o.Value.Field.IsMap && !o.Value.Field.IsFun && o.Value.Field.InsertStatusType == StatusType.CanWrite))
            {
                var obj = PropertyGetCacheManger.Cache(kic.Key, entity);
                if (obj == null) { continue; }
                var member = Expression.MakeMemberAccess(oParameter, kic.Key);

                // 主键、只读条件、主键状态下，转为条件状态
                if ((kic.Value.Field.UpdateStatusType == StatusType.ReadCondition || kic.Value.Field.IsPrimaryKey))
                {
                    // 当前条件已存在该值时，跳过
                    if (new GetMemberVisitor().Visit(ExpWhere).Any(o => o.Member == kic.Key)) { continue; }
                    var expCondiction = ExpressionHelper.CreateBinaryExpression<TEntity>(obj, member);
                    ExpWhere = ExpressionHelper.MergeAndAlsoExpression((Expression<Func<TEntity, bool>>) ExpWhere, expCondiction);
                }
                else
                {
                    var ass = Expression.Assign(member, Expression.Convert(Expression.Constant(obj), kic.Key.PropertyType));
                    lstAssign.Add(ass);
                }
            }
            ExpAssign = ExpressionHelper.MergeBlockExpression(lstAssign.ToArray());
        }

        /// <summary>
        ///     查询数据时，如果设置了逻辑删除，则要过滤数据
        /// </summary>
        internal void DeleteSortCondition()
        {
            if (SetMap.SortDelete != null) { AddWhere(SetMap.SortDelete.CondictionExpression); }
        }
    }
}