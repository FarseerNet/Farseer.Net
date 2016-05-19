using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace FS.Utils.Common.ExpressionVisitor
{
    /// <summary>
    ///     对表达式树的解析
    /// </summary>
    public abstract class AbsExpressionVisitor
    {
        /// <summary>
        ///     解析入口
        /// </summary>
        /// <param name="exp">传入解释的表达式树</param>
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null) { return null; }
            switch (exp.NodeType)
            {
                case ExpressionType.ListInit:
                case ExpressionType.Call:
                case ExpressionType.Constant:
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                case ExpressionType.NewArrayInit:
                    exp = VisitConvertExp(exp);
                    break;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.UnaryPlus:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.OrAssign:
                case ExpressionType.Assign:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                case ExpressionType.Block:
                    return this.VisitBlock((BlockExpression)exp);
            }
            throw new Exception($"类型：(ExpressionType){exp.NodeType}，不存在。");
        }

        protected virtual Expression VisitBlock(BlockExpression block)
        {
            foreach (var exp in block.Expressions) { this.Visit(exp); }
            return block;
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            if (b == null) { return null; }
            var isReverse = false;
            var left = b.Left;
            var right = b.Right;

            // 先解析字段
            if (b.Left.NodeType != ExpressionType.MemberAccess && (b.Left.NodeType == ExpressionType.MemberAccess || b.Right.NodeType == ExpressionType.MemberAccess))
            {
                left = b.Right;
                right = b.Left;
                isReverse = true;
            }
            left = this.Visit(left);
            right = this.Visit(right);
            var conversion = this.Visit(b.Conversion);

            var contidion = isReverse ? (left != b.Right || right != b.Left) : (left != b.Left || right != b.Right);
            // 说明进行了换算 需要重新生成
            if (contidion || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null) { return Expression.Coalesce(left, right, conversion as LambdaExpression); }
                else
                {
                    // 两边类型不同时，需要进行转换
                    if (left.Type != right.Type) { right = Expression.Convert(right, left.Type); }
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
                }
            }
            return b;
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            var obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments) { return Expression.Call(obj, m.Method, args); }
            return m;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected virtual Expression VisitConstant(ConstantExpression cexp)
        {
            return cexp;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            var exp = this.Visit(m.Expression);
            if (exp != m.Expression) { return Expression.MakeMemberAccess(exp, m.Member); }
            return m;

            //if (m == null) return null;
            //if (m.NodeType == ExpressionType.Constant) { return this.Visit(VisitConvertExp(m)); }
            //return m;
        }

        /// <summary>
        ///     值类型的转换
        /// </summary>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            var operand = this.Visit(u.Operand);
            if (operand != u.Operand) { return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method); }
            return u;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            var body = this.Visit(lambda.Body);
            if (body != lambda.Body) { return Expression.Lambda(lambda.Type, body, lambda.Parameters); }
            return lambda;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            var expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression) { return Expression.Invoke(expr, args); }
            return iv;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = this.Visit(original[i]);
                if (list != null) { list.Add(p); }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (var j = 0; j < i; j++) { list.Add(original[j]); }
                    list.Add(p);
                }
            }
            if (list != null) { return list.AsReadOnly(); }
            return original;
        }

        /// <summary>
        ///     将变量转换成值
        /// </summary>
        protected virtual Expression VisitConvertExp(Expression exp)
        {
            var u = exp as UnaryExpression;
            if (u != null && u.Operand is ConstantExpression) { return u.Operand; }
            if (exp is BinaryExpression || !IsFieldValue(exp)) { return exp; }
            try { return Expression.Constant(Expression.Lambda(exp).Compile().DynamicInvoke(null), exp.Type); }
            catch
            {
                throw new Exception($"表达式树类型转换失败，对象({((MemberExpression)exp).Expression.Type})为不能为Null类型。");
            }
        }

        /// <summary>
        ///     判断是字段，还是值类型
        /// </summary>
        protected virtual bool IsFieldValue(Expression exp)
        {
            // 尝试通过获取Parameter来判断
            if (exp == null) { return false; }
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return ((LambdaExpression)exp).Parameters.Count == 0 && IsFieldValue(((LambdaExpression)exp).Body);
                case ExpressionType.Call:
                    {
                        var callExp = (MethodCallExpression)exp;
                        if (callExp.Object != null && !IsFieldValue(callExp.Object)) { return false; }
                        return callExp.Arguments.All(IsFieldValue);
                    }
                case ExpressionType.MemberAccess:
                    {
                        var memExp = (MemberExpression)exp;
                        return memExp.Expression == null || IsFieldValue(memExp.Expression);
                    }
                case ExpressionType.Parameter:
                    return !exp.Type.IsClass && !exp.Type.IsAbstract && !exp.Type.IsInterface;
                case ExpressionType.Convert:
                    return IsFieldValue(((UnaryExpression)exp).Operand);
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    return IsFieldValue(((BinaryExpression)exp).Left) && IsFieldValue(((BinaryExpression)exp).Right);
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

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments) { return Expression.New(nex.Constructor, args, nex.Members); }
            return nex;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit) { return Expression.NewArrayInit(na.Type.GetElementType(), exprs); }
                else
                { return Expression.NewArrayBounds(na.Type.GetElementType(), exprs); }
            }
            return na;
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var b = this.VisitBinding(original[i]);
                if (list != null) { list.Add(b); }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (var j = 0; j < i; j++) { list.Add(original[j]); }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments) { return Expression.ElementInit(initializer.AddMethod, arguments); }
            return initializer;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var init = this.VisitElementInitializer(original[i]);
                if (list != null) { list.Add(init); }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (var j = 0; j < i; j++) { list.Add(original[j]); }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = this.Visit(assignment.Expression);
            if (e != assignment.Expression) { return Expression.Bind(assignment.Member, e); }
            return assignment;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers) { return Expression.ListBind(binding.Member, initializers); }
            return binding;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings) { return Expression.MemberBind(binding.Member, bindings); }
            return binding;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expr = this.Visit(b.Expression);
            if (expr != b.Expression) { return Expression.TypeIs(expr, b.TypeOperand); }
            return b;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = this.Visit(c.Test);
            var ifTrue = this.Visit(c.IfTrue);
            var ifFalse = this.Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse) { return Expression.Condition(test, ifTrue, ifFalse); }
            return c;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            var n = this.VisitNew(init.NewExpression);
            var bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings) { return Expression.MemberInit(n, bindings); }
            return init;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            var n = this.VisitNew(init.NewExpression);
            var initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers) { return Expression.ListInit(n, initializers); }
            return init;
        }
    }
}