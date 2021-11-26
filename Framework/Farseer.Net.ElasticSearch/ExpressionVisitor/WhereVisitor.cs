using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            if (_queryList.Count == 0) return null;

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
            if (exp == null) return null;
            if (_curValue == null || _curValue.Count == 0) return exp;

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
                    query = new QueryContainerDescriptor<TDocument>().Bool(b => b.MustNot(m => m.Term(field, value)));
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
        ///     将属性变量转换成属性字段名Field Name
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return null;
            _curField.Push(Expression.Lambda(m, (ParameterExpression)m.Expression));
            return m;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成字段值Value
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression cexp)
        {
            base.VisitConstant(cexp);
            _curValue.Push(cexp.Value);
            return cexp;
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (IsIgnoreMethod(m: m)) return m;

            var methodObject = m.Object;
            var arguments    = m.Arguments.ToList();
            if (methodObject == null)
            {
                methodObject = arguments[index: 0];
                arguments.RemoveAt(index: 0);
            }

            // 解析字段、值
            base.Visit(exp: methodObject);

            if (_curField.Count == 1 && _curValue.Count == 1)
            {
                return m;
            }

            foreach (var t in arguments) base.Visit(exp: t);

            var field = _curField.Pop();
            var value = _curValue.Pop();
            if (value == null) return m;

            var lambdaExpression = (LambdaExpression)field;
            var memberInfo       = ((MemberExpression)lambdaExpression.Body).Member;
            // keyword类型，则使用Wildcard匹配，Text类型，则使用Match
            var isKeyword = memberInfo.IsDefined(typeof(KeywordAttribute));

            QueryContainer query;
            switch (m.Method.Name)
            {
                case "Contains":
                    if (isKeyword)
                    {
                        query = new QueryContainerDescriptor<TDocument>().Wildcard(field, $"*{value}*");
                    }
                    else
                    {
                        query = new QueryContainerDescriptor<TDocument>().Match(m => m.Field(field).Query(value.ToString()));
                    }
                    _queryList.Push(query);
                    break;
                case "StartsWith":
                    if (isKeyword)
                    {
                        query = new QueryContainerDescriptor<TDocument>().Wildcard(field, $"{value}*");
                    }
                    else
                    {
                        query = new QueryContainerDescriptor<TDocument>().MatchPhrasePrefix(m => m.Field(field).Query(value.ToString()));
                    }
                    _queryList.Push(query);
                    break;
                case "EndsWith":
                    if (isKeyword)
                    {
                        query = new QueryContainerDescriptor<TDocument>().Wildcard(field, $"*{value}");
                    }
                    else
                    {
                        throw new Exception(message: string.Format(format: "暂不支持该方法的Test类型的ES搜索：" + m.Method.Name.ToUpper()));
                    }
                    _queryList.Push(query);
                    break;
                case "IsEquals":
                case "Equals":
                    query = new QueryContainerDescriptor<TDocument>().Term(field, value);
                    _queryList.Push(query);
                    break;
                case "ToShortDate":
                    break;
                default:
                {
                    if (m.Arguments.Count == 0 && m.Object != null) return m;
                    throw new Exception(message: string.Format(format: "暂不支持该方法的ES搜索：" + m.Method.Name.ToUpper()));
                }
            }
            return m;
        }

        /// <summary>
        ///     判断是字段，还是值类型
        /// </summary>
        protected override bool IsFieldValue(Expression exp)
        {
            // 尝试通过获取Parameter来判断
            if (exp == null) return false;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return ((LambdaExpression)exp).Parameters.Count == 0 && IsFieldValue(exp: ((LambdaExpression)exp).Body);
                case ExpressionType.Call:
                {
                    var callExp = (MethodCallExpression)exp;
                    return IsFieldValue(callExp.Object ?? callExp.Arguments[0]);
                }
                case ExpressionType.MemberAccess:
                {
                    var memExp = (MemberExpression)exp;
                    return memExp.Expression == null || IsFieldValue(exp: memExp.Expression);
                }
                case ExpressionType.Parameter: return !exp.Type.IsClass && !exp.Type.GetTypeInfo().IsAbstract && !exp.Type.GetTypeInfo().IsInterface;
                case ExpressionType.Convert:   return IsFieldValue(exp: ((UnaryExpression)exp).Operand);
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide: return IsFieldValue(exp: ((BinaryExpression)exp).Left) && IsFieldValue(exp: ((BinaryExpression)exp).Right);
                case ExpressionType.ArrayIndex:
                case ExpressionType.ListInit:
                case ExpressionType.Constant:
                case ExpressionType.NewArrayInit:
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     忽略字段的方法
        /// </summary>
        protected virtual bool IsIgnoreMethod(MethodCallExpression m)
        {
            if (m.Method.DeclaringType.FullName == "FS.Extends.Extend") return true;

            switch (m.Method.Name)
            {
                case "ConvertType":
                    if (Assembly.GetAssembly(type: m.Method.ReflectedType) == Assembly.GetCallingAssembly())
                    {
                        base.Visit(exp: m.Object ?? m.Arguments[index: 0]);
                        return true;
                    }

                    return false;
                case "ToDateTime": return true;
                default:           return false;
            }
        }
    }
}