using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Collections.Pooled;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中的参数变量
    /// </summary>
    public class GetBinaryVisitor : AbsExpressionVisitor
    {
        private readonly PooledList<Expression> _lst = new();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public new PooledList<Expression> Visit(Expression exp)
        {
            base.Visit(exp: exp);
            return _lst;
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            _lst.Add(item: b);
            return b;
        }
    }
}