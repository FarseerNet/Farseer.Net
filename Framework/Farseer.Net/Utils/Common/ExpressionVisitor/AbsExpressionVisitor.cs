using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        /// <param name="exp"> 传入解释的表达式树 </param>
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null) return null;
            switch (exp.NodeType)
            {
                case ExpressionType.ListInit:
                case ExpressionType.Call:
                case ExpressionType.Constant:
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                case ExpressionType.NewArrayInit:
                    exp = VisitConvertExp(exp: exp);
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
                case ExpressionType.TypeAs: return VisitUnary(u: (UnaryExpression)exp);
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
                case ExpressionType.ExclusiveOr: return VisitBinary(b: (BinaryExpression)exp);
                case ExpressionType.TypeIs:       return VisitTypeIs(b: (TypeBinaryExpression)exp);
                case ExpressionType.Conditional:  return VisitConditional(c: (ConditionalExpression)exp);
                case ExpressionType.Constant:     return VisitConstant(cexp: (ConstantExpression)exp);
                case ExpressionType.Parameter:    return VisitParameter(p: (ParameterExpression)exp);
                case ExpressionType.MemberAccess: return VisitMemberAccess(m: (MemberExpression)exp);
                case ExpressionType.Call:         return VisitMethodCall(m: (MethodCallExpression)exp);
                case ExpressionType.Lambda:       return VisitLambda(lambda: (LambdaExpression)exp);
                case ExpressionType.New:          return VisitNew(nex: (NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds: return VisitNewArray(na: (NewArrayExpression)exp);
                case ExpressionType.Invoke:     return VisitInvocation(iv: (InvocationExpression)exp);
                case ExpressionType.MemberInit: return VisitMemberInit(init: (MemberInitExpression)exp);
                case ExpressionType.ListInit:   return VisitListInit(init: (ListInitExpression)exp);
                case ExpressionType.Block:      return VisitBlock(block: (BlockExpression)exp);
            }

            throw new Exception(message: $"类型：(ExpressionType){exp.NodeType}，不存在。");
        }

        /// <summary>
        ///     表达式树块
        /// </summary>
        /// <param name="block"> </param>
        /// <returns> </returns>
        protected virtual Expression VisitBlock(BlockExpression block)
        {
            foreach (var exp in block.Expressions) Visit(exp: exp);
            return block;
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            if (b == null) return null;
            var isReverse = false;
            var left      = b.Left;
            var right     = b.Right;

            // 先解析字段
            if (b.Left.NodeType != ExpressionType.MemberAccess && (b.Left.NodeType == ExpressionType.MemberAccess || b.Right.NodeType == ExpressionType.MemberAccess))
            {
                left      = b.Right;
                right     = b.Left;
                isReverse = true;
            }

            left  = Visit(exp: left);
            right = Visit(exp: right);
            var conversion = Visit(exp: b.Conversion);

            var contidion = isReverse ? left != b.Right || right != b.Left : left != b.Left || right != b.Right;
            // 说明进行了换算 需要重新生成
            if (contidion || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null) return Expression.Coalesce(left: left, right: right, conversion: conversion as LambdaExpression);
                // 两边类型不同时，需要进行转换
                if (left.Type != right.Type) right = Expression.Convert(expression: right, type: left.Type);
                return Expression.MakeBinary(binaryType: b.NodeType, left: left, right: right, liftToNull: b.IsLiftedToNull, method: b.Method);
            }

            return b;
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            var                     obj  = Visit(exp: m.Object);
            IEnumerable<Expression> args = VisitExpressionList(original: m.Arguments);
            if (obj != m.Object || args != m.Arguments) return Expression.Call(instance: obj, method: m.Method, arguments: args);
            return m;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected virtual Expression VisitConstant(ConstantExpression cexp) => cexp;

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            var exp = Visit(exp: m.Expression);
            if (exp != m.Expression) return Expression.MakeMemberAccess(expression: exp, member: m.Member);
            return m;
        }

        /// <summary>
        ///     值类型的转换
        /// </summary>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            var operand = Visit(exp: u.Operand);
            if (operand != u.Operand) return Expression.MakeUnary(unaryType: u.NodeType, operand: operand, type: u.Type, method: u.Method);
            return u;
        }

        /// <summary>
        ///     Lambda表达式
        /// </summary>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            var body = Visit(exp: lambda.Body);
            if (body != lambda.Body) return Expression.Lambda(delegateType: lambda.Type, body: body, parameters: lambda.Parameters);
            return lambda;
        }

        /// <summary>
        ///     自变量表达式列表的表达式
        /// </summary>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = VisitExpressionList(original: iv.Arguments);
            var                     expr = Visit(exp: iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression) return Expression.Invoke(expression: expr, arguments: args);
            return iv;
        }

        /// <summary>
        ///     解析多个表达式树
        /// </summary>
        /// <param name="original"> </param>
        /// <returns> </returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0,
                     n = original.Count; i < n; i++)
            {
                var p = Visit(exp: original[index: i]);
                if (list != null)
                    list.Add(item: p);
                else if (p != original[index: i])
                {
                    list = new List<Expression>(capacity: n);
                    for (var j = 0; j < i; j++) list.Add(item: original[index: j]);
                    list.Add(item: p);
                }
            }

            if (list != null) return list.AsReadOnly();
            return original;
        }

        /// <summary>
        ///     将变量转换成值
        /// </summary>
        protected virtual Expression VisitConvertExp(Expression exp)
        {
            if (exp is UnaryExpression
            {
                Operand: ConstantExpression
            } u)
                return u.Operand;
            if (exp is BinaryExpression || !IsFieldValue(exp: exp)) return exp;
            try
            {
                return Expression.Constant(value: Expression.Lambda(body: exp).Compile().DynamicInvoke(args: null), type: exp.Type);
            }
            catch
            {
                throw new Exception(message: $"表达式树类型转换失败，对象({((MemberExpression)exp).Expression.Type})为不能为Null类型。");
            }
        }

        /// <summary>
        ///     判断是字段，还是值类型
        /// </summary>
        protected virtual bool IsFieldValue(Expression exp)
        {
            // 尝试通过获取Parameter来判断
            if (exp == null) return false;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda: return ((LambdaExpression)exp).Parameters.Count == 0 && IsFieldValue(exp: ((LambdaExpression)exp).Body);
                case ExpressionType.Call:
                {
                    var callExp = (MethodCallExpression)exp;
                    if (callExp.Object != null)
                    {
                        return IsFieldValue(exp: callExp.Object);
                    }
                    else
                    {
                        return IsFieldValue(exp: callExp.Arguments[0]);
                    }
                    
                    //return callExp.Arguments.All(predicate: IsFieldValue);
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
        ///     参数表达式
        /// </summary>
        protected virtual Expression VisitParameter(ParameterExpression p) => p;

        /// <summary>
        ///     构造函数表达式
        /// </summary>
        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = VisitExpressionList(original: nex.Arguments);
            if (args != nex.Arguments) return Expression.New(constructor: nex.Constructor, arguments: args, members: nex.Members);
            return nex;
        }

        /// <summary>
        ///     多个构造函数表达式
        /// </summary>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = VisitExpressionList(original: na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit) return Expression.NewArrayInit(type: na.Type.GetElementType(), initializers: exprs);
                return Expression.NewArrayBounds(type: na.Type.GetElementType(), bounds: exprs);
            }

            return na;
        }

        /// <summary>
        ///     将MemberBinding类型转换到所属成员类型上
        /// </summary>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:    return VisitMemberAssignment(assignment: (MemberAssignment)binding);
                case MemberBindingType.MemberBinding: return VisitMemberMemberBinding(binding: (MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:   return VisitMemberListBinding(binding: (MemberListBinding)binding);
                default:                              throw new Exception(message: $"Unhandled binding type '{binding.BindingType}'");
            }
        }

        /// <summary>
        ///     将MemberBinding类型转换到所属成员类型上
        /// </summary>
        protected IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0,
                     n = original.Count; i < n; i++)
            {
                var b = VisitBinding(binding: original[index: i]);
                if (list != null)
                    list.Add(item: b);
                else if (b != original[index: i])
                {
                    list = new List<MemberBinding>(capacity: n);
                    for (var j = 0; j < i; j++) list.Add(item: original[index: j]);
                    list.Add(item: b);
                }
            }

            if (list != null) return list;
            return original;
        }

        /// <summary>
        ///     表示 IEnumerable 集合的单个元素的初始值设定项。
        /// </summary>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(original: initializer.Arguments);
            if (arguments != initializer.Arguments) return Expression.ElementInit(addMethod: initializer.AddMethod, arguments: arguments);
            return initializer;
        }

        /// <summary>
        ///     表示 IEnumerable 集合的多个元素的初始值设定项。
        /// </summary>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0,
                     n = original.Count; i < n; i++)
            {
                var init = VisitElementInitializer(initializer: original[index: i]);
                if (list != null)
                    list.Add(item: init);
                else if (init != original[index: i])
                {
                    list = new List<ElementInit>(capacity: n);
                    for (var j = 0; j < i; j++) list.Add(item: original[index: j]);
                    list.Add(item: init);
                }
            }

            if (list != null) return list;
            return original;
        }

        /// <summary>
        ///     对象的字段或属性的赋值操作。
        /// </summary>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(exp: assignment.Expression);
            if (e != assignment.Expression) return Expression.Bind(member: assignment.Member, expression: e);
            return assignment;
        }

        /// <summary>
        ///     初始化新创建对象的一个集合成员的元素。
        /// </summary>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(original: binding.Initializers);
            if (initializers != binding.Initializers) return Expression.ListBind(member: binding.Member, initializers: initializers);
            return binding;
        }

        /// <summary>
        ///     初始化新创建对象的一个成员的成员。
        /// </summary>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(original: binding.Bindings);
            if (bindings != binding.Bindings) return Expression.MemberBind(member: binding.Member, bindings: bindings);
            return binding;
        }

        /// <summary>
        ///     表达式和类型之间的操作。
        /// </summary>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expr = Visit(exp: b.Expression);
            if (expr != b.Expression) return Expression.TypeIs(expression: expr, type: b.TypeOperand);
            return b;
        }

        /// <summary>
        ///     具有条件运算符的表达式。
        /// </summary>
        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test    = Visit(exp: c.Test);
            var ifTrue  = Visit(exp: c.IfTrue);
            var ifFalse = Visit(exp: c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse) return Expression.Condition(test: test, ifTrue: ifTrue, ifFalse: ifFalse);
            return c;
        }

        /// <summary>
        ///     调用构造函数并初始化新对象的一个或多个成员。
        /// </summary>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            var n        = VisitNew(nex: init.NewExpression);
            var bindings = VisitBindingList(original: init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings) return Expression.MemberInit(newExpression: n, bindings: bindings);
            return init;
        }

        /// <summary>
        ///     具有集合初始值设定项的构造函数调用。
        /// </summary>
        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            var n            = VisitNew(nex: init.NewExpression);
            var initializers = VisitElementInitializerList(original: init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers) return Expression.ListInit(newExpression: n, initializers: initializers);
            return init;
        }
    }
}