using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中的参数变量
    /// </summary>
    public class GetBinaryVisitor : AbsExpressionVisitor
    {
        private readonly List<Expression> _lst = new List<Expression>();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public new IEnumerable<Expression> Visit(Expression exp)
        {
            base.Visit(exp);
            return _lst;
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            _lst.Add(b);
            return b;
        }
    }
}