using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Collections.Pooled;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     获取表达式中的参数变量
    /// </summary>
    public class GetParamVisitor : AbsExpressionVisitor
    {
        private readonly PooledList<ParameterExpression> _lst = new();

        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public new PooledList<ParameterExpression> Visit(Expression exp)
        {
            base.Visit(exp: exp);
            return _lst;
        }
        
        /// <summary>
        ///     获取表达式中的变量
        /// </summary>
        public ParameterExpression VisitAndReturnFirst(Expression exp)
        {
            base.Visit(exp: exp);
            using (_lst)
            {
                return _lst.FirstOrDefault();
            }
        }

        /// <summary>
        ///     把参数表达式树添加到列表中
        /// </summary>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_lst.Exists(match: o => o.Name == p.Name && o.Type == p.Type)) return p;
            _lst.Add(item: p);
            return p;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            base.VisitMemberAccess(m: m);
            return m;
        }
    }
}