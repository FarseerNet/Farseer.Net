using System.Collections.Generic;
using System.Linq.Expressions;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中的参数变量
    /// </summary>
    public class GetParamVisitor : AbsExpressionVisitor
    {
        private readonly List<ParameterExpression> _lst = new List<ParameterExpression>();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public new IEnumerable<ParameterExpression> Visit(Expression exp)
        {
            base.Visit(exp);
            return _lst;
        }

        /// <summary>
        ///     把参数表达式树添加到列表中
        /// </summary>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_lst.Exists(o => o.Name == p.Name && o.Type == p.Type)) { return p; }
            _lst.Add(p);
            return p;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            base.VisitMemberAccess(m);
            return m;
        }
    }
}