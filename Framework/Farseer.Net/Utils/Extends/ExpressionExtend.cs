using System;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     Expression表达式树扩展
    /// </summary>
    public static partial class ExpressionExtend
    {
        /// <summary>
        ///     获取字段
        /// </summary>
        /// <param name="select"> 字段名称 </param>
        /// <returns> </returns>
        public static PropertyInfo GetPropertyInfo<T1, T2>(this Expression<Func<T1, T2>> select) where T1 : class
        {
            MemberExpression memberExpression;

            if (select.Body is UnaryExpression unary)
                memberExpression = unary.Operand as MemberExpression;
            else if (select.Body.NodeType == ExpressionType.Call)
                memberExpression = (MemberExpression)((MethodCallExpression)select.Body).Object;
            else
                memberExpression = select.Body as MemberExpression;

            return (PropertyInfo)memberExpression.Member;
        }
    }
}