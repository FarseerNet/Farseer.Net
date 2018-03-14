using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     Expression表达式树扩展
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     And 操作
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(this Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            return ExpressionHelper.MergeAndAlsoExpression(left, right);
        }

        /// <summary>
        ///     OR 操作
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TEntity, bool>> OrElse<TEntity>(this Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right) where TEntity : class
        {
            return ExpressionHelper.MergeOrElseExpression(left, right);
        }


        /// <summary>
        /// 合并两个表达式树
        /// </summary>
        public static Expression<Func<TOuter, TInner>> Combine<TOuter, TMiddle, TInner>(Expression<Func<TOuter, TMiddle>> first, Expression<Func<TMiddle, TInner>> second)
        {
            return x => second.Compile()(first.Compile()(x));
        }

        /// <summary>
        ///     获取类属性成员名称
        /// </summary>
        /// <param name="select">select表达式树</param>
        /// <returns></returns>
        public static List<MemberExpression> GetMemberExpression(this Expression select)
        {
            switch (select.NodeType)
            {
                case ExpressionType.New:
                {
                    return ((NewExpression) select).Arguments.Select(o => (MemberExpression) o).ToList();
                }
                case ExpressionType.Lambda:
                    return GetMemberExpression(((LambdaExpression) select).Body);
                case ExpressionType.MemberAccess:
                {
                    return new List<MemberExpression> {((MemberExpression) select)};
                }
                    ;
            }
            ;
            return new List<MemberExpression>();
        }
    }
}