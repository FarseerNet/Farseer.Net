using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Extends;
using FS.Utils.Common.ExpressionVisitor;
using Nest;

namespace FS.ElasticSearch.ExpressionVisitor
{
    /// <summary>
    ///     提供ExpressionBinary表达式树的解析
    /// </summary>
    public class WhereVisitor<TDocument> : AbsExpressionVisitor where TDocument : class, new()
    {
        private readonly Stack<Expression> _curField = new();
        private readonly Stack<object>     _curValue = new();

        private readonly Stack<QueryContainer> _queryList = new();
        public new QueryContainer Visit(Expression exp)
        {
            base.Visit(exp: exp);
            var query = _queryList.Pop();

            while (_queryList.Count > 0)
            {
                query = query && _queryList.Pop();
            }
            return query;
        }

        /// <summary>
        ///     将二元符号转换成ES可识别的操作符
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression bexp)
        {
            var exp = base.VisitBinary(b: bexp);
            if (exp       == null) return null;
            if (_curValue == null) return exp;

            // 如果是比较值的操作，则先取出字段、值
            Expression field = null;
            object     value = null;
            switch (exp.NodeType)
            {
                case ExpressionType.NotEqual:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    field = _curField.Pop();
                    value = _curValue.Pop();
                    if (value == null) return exp;
                    break;
            }

            QueryContainer query;
            switch (exp.NodeType)
            {
                case ExpressionType.NotEqual:
                    query = new QueryContainerDescriptor<TDocument>().Bool(b=>b.MustNot(m=>m.Term(field, value)));
                    _queryList.Push(query);
                    break;
                case ExpressionType.Equal:
                    query = new QueryContainerDescriptor<TDocument>().Term(field, value);
                    _queryList.Push(query);
                    break;
                case ExpressionType.GreaterThan:
                    query = new QueryContainerDescriptor<TDocument>().LongRange(l => l.Field(field).GreaterThan(value.ConvertType(0L)));
                    _queryList.Push(query);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    query = new QueryContainerDescriptor<TDocument>().LongRange(l => l.Field(field).GreaterThanOrEquals(value.ConvertType(0L)));
                    _queryList.Push(query);
                    break;
                case ExpressionType.LessThan:
                    query = new QueryContainerDescriptor<TDocument>().LongRange(l => l.Field(field).LessThan(value.ConvertType(0L)));
                    _queryList.Push(query);
                    break;
                case ExpressionType.LessThanOrEqual:
                    query = new QueryContainerDescriptor<TDocument>().LongRange(l => l.Field(field).LessThanOrEquals(value.ConvertType(0L)));
                    _queryList.Push(query);
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _queryList.Push(_queryList.Pop() || _queryList.Pop());
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _queryList.Push(_queryList.Pop() && _queryList.Pop());
                    break;
            }
            return exp;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return null;
            _curField.Push(Expression.Lambda(m, (ParameterExpression)m.Expression));
            return m;
        }


        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression cexp)
        {
            base.VisitConstant(cexp: cexp);
            _curValue.Push(cexp.Value);
            return cexp;
        }
    }
}