using System;
using System.Linq.Expressions;
using FS.Data.Cache;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     Expression表达式树扩展
    /// </summary>
    public static partial class SqlExtend
    {
        /// <summary>
        ///     获取字段名称
        /// </summary>
        /// <param name="select"> 字段名称 </param>
        /// <returns> </returns>
        public static string GetUsedName<T1, T2>(this Expression<Func<T1, T2>> select) where T1 : class
        {
            MemberExpression memberExpression;

            var unary = select.Body as UnaryExpression;
            if (unary != null)
                memberExpression = unary.Operand as MemberExpression;
            else if (select.Body.NodeType == ExpressionType.Call)
                memberExpression = (MemberExpression)((MethodCallExpression)select.Body).Object;
            else
                memberExpression = select.Body as MemberExpression;

            var map       = SetMapCacheManger.Cache(key: typeof(T1));
            var modelInfo = map.GetState(propertyName: memberExpression.Member.Name);

            return modelInfo.Value.Field.Name;
        }
    }
}