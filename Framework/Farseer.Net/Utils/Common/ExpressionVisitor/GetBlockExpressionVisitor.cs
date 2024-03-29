﻿using System.Linq.Expressions;
using Collections.Pooled;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中Block的Expression
    /// </summary>
    public class GetBlockExpressionVisitor : AbsExpressionVisitor
    {
        private readonly PooledList<Expression> _lst = new();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public new PooledList<Expression> Visit(Expression exp)
        {
            base.Visit(exp: exp);
            if (_lst.Count == 0 && exp != null) _lst.Add(item: exp);
            return _lst;
        }

        /// <summary>
        ///     解析表达式树块
        /// </summary>
        protected override Expression VisitBlock(BlockExpression block)
        {
            _lst.AddRange(collection: block.Expressions);
            return block;
        }
    }
}