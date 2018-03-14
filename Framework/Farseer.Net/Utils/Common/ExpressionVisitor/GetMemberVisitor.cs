using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中的成员
    /// </summary>
    public class GetMemberVisitor : AbsExpressionVisitor
    {
        private readonly List<MemberExpression> _lst = new List<MemberExpression>();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public IEnumerable<MemberExpression> Visit(params Expression[] exps)
        {
            foreach (var exp in exps) { base.Visit(exp); }
            return _lst;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            base.VisitMemberAccess(m);
            if (_lst.Exists(o => o.Member.Name == m.Member.Name && o.Type == m.Type)) { return m; }
            _lst.Add(m);
            return m;
        }
    }
}