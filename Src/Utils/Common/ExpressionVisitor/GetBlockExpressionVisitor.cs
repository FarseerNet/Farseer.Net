using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中Block的Expression
    /// </summary>
    public class GetBlockExpressionVisitor : AbsExpressionVisitor
    {
        private readonly List<Expression> _lst = new List<Expression>();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public new IEnumerable<Expression> Visit(Expression exp)
        {
            base.Visit(exp);
            if (_lst.Count == 0 && exp != null) { _lst.Add(exp); }
            return _lst;
        }

        protected override Expression VisitBlock(BlockExpression block)
        {
            _lst.AddRange(block.Expressions);
            return block;
        }
    }
}