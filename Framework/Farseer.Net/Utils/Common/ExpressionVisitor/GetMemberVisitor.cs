using System.Collections.Generic;
using System.Linq.Expressions;
using Collections.Pooled;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中的成员
    /// </summary>
    public class GetMemberVisitor : AbsExpressionVisitor
    {
        private readonly PooledList<MemberExpression> _lst = new();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public PooledList<MemberExpression> Visit(params Expression[] exps)
        {
            foreach (var exp in exps) base.Visit(exp: exp);
            return _lst;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            base.VisitMemberAccess(m: m);
            if (_lst.Exists(match: o => o.Member.Name == m.Member.Name && o.Type == m.Type)) return m;
            _lst.Add(item: m);
            return m;
        }
    }
}