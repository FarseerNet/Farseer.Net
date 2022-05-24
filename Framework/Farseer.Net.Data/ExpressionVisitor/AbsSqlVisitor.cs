using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FS.Core.Mapping;
using FS.Core.Mapping.Attribute;
using FS.Data.Client;
using FS.Data.Map;
using FS.Extends;
using FS.Utils.Common.ExpressionVisitor;
using Newtonsoft.Json;

namespace FS.Data.ExpressionVisitor
{
    /// <summary>
    ///     提供ExpressionBinary表达式树的解析
    /// </summary>
    public abstract class AbsSqlVisitor : AbsExpressionVisitor
    {
        /// <summary>
        ///     条件堆栈
        /// </summary>
        protected readonly Stack<string> SqlList = new();

        /// <summary>
        ///     当前值参数
        /// </summary>
        protected DbParameter CurrentDbParameter;

        /// <summary>
        ///     当前字段
        /// </summary>
        protected KeyValuePair<PropertyInfo, DbFieldMapState> CurrentField;

        /// <summary>
        ///     当前字段名称
        ///     与CurrentField区别是，在调用PO类中的属性的子属性时，CurrentField有可能是无法获取的。所以才有了CurrentFieldName
        /// </summary>
        protected string CurrentFieldName;

        /// <summary>
        ///     是否包括Not条件
        /// </summary>
        protected bool IsNot;

        /// <summary>
        ///     默认构造器
        /// </summary>
        /// <param name="dbProvider"> 数据库提供者（不同数据库的特性） </param>
        /// <param name="map"> 字段映射 </param>
        /// <param name="paramList"> SQL参数列表 </param>
        protected AbsSqlVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList)
        {
            DbProvider = dbProvider;
            SetMap     = map;
            ParamList  = paramList;
        }

        /// <summary>
        ///     SQL参数列表
        /// </summary>
        protected List<DbParameter> ParamList { get; }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        protected AbsDbProvider DbProvider { get; }

        /// <summary>
        ///     数据库函数提供者（不同数据库的函数）
        /// </summary>
        protected AbsFunctionProvider FunctionProvider => DbProvider.FunctionProvider;

        /// <summary>
        ///     字段映射
        /// </summary>
        private SetDataMap SetMap { get; }

        /// <summary>
        ///     清除当前所有数据
        /// </summary>
        public void Clear()
        {
            SqlList.Clear();
            CurrentFieldName   = null;
            CurrentDbParameter = null;
            ParamList.Clear();
            IsNot = false;
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression bexp)
        {
            var exp = base.VisitBinary(b: bexp);
            if (exp == null) return null;

            var right = SqlList.Pop();
            var left  = SqlList.Pop();

            if (bexp.NodeType == ExpressionType.AndAlso || bexp.NodeType == ExpressionType.OrElse)
            {
                right = SqlTrue(sql: right);
                left  = SqlTrue(sql: left);
            }

            if (CurrentDbParameter is
                {
                    Value: null
                })
            {
                right = "NULL";
                ParamList.RemoveAt(index: ParamList.Count - 1);
            }

            VisitOperate(bexp: bexp, left: left, right: right);

            // 清除状态（与或状态，不清除）
            if (bexp.NodeType != ExpressionType.And && bexp.NodeType != ExpressionType.Or)
            {
                CurrentFieldName   = null;
                CurrentDbParameter = null;
            }

            return exp;
        }

        protected virtual string VisitOperate(BinaryExpression bexp, string left)
        {
            switch (bexp.NodeType)
            {
                case ExpressionType.Assign:             return "=";
                case ExpressionType.Equal:              return CurrentDbParameter?.Value != null || bexp.Left.NodeType == ExpressionType.MemberAccess && bexp.Right.NodeType == ExpressionType.MemberAccess ? "=" : "IS";
                case ExpressionType.NotEqual:           return CurrentDbParameter?.Value != null || bexp.Left.NodeType == ExpressionType.MemberAccess && bexp.Right.NodeType == ExpressionType.MemberAccess ? "<>" : "IS NOT"; //|| bexp.Left is PropertyExpression
                case ExpressionType.GreaterThan:        return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan:           return "<";
                case ExpressionType.LessThanOrEqual:    return "<=";
                case ExpressionType.AndAlso:            return "AND";
                case ExpressionType.OrElse:             return "OR";
                case ExpressionType.Add:                return "+";
                case ExpressionType.Subtract:           return "-";
                case ExpressionType.Multiply:           return "*";
                case ExpressionType.Divide:             return "/";
                case ExpressionType.And:                return "&";
                case ExpressionType.Or:                 return "|";
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked: return $"= {left} + ";
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PreIncrementAssign: return $"= {left} + 1";
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.PreDecrementAssign: return $"= {left} - 1 ";
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked: return $"= {left} - ";
                case ExpressionType.AndAssign:    return $"= {left} & ";
                case ExpressionType.DivideAssign: return $"= {left} / ";
                case ExpressionType.PowerAssign:
                case ExpressionType.ExclusiveOrAssign: return $"= {left} ^ ";
                case ExpressionType.LeftShiftAssign:  return $"= {left} << ";
                case ExpressionType.RightShiftAssign: return $"= {left} >> ";
                case ExpressionType.ModuloAssign:     return $"= {left} % ";
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked: return $"= {left} * ";
                case ExpressionType.OrAssign: return $"= {left} | ";
                default:                      throw new NotSupportedException(message: bexp.NodeType + "的类型，未定义操作符号！");
            }
        }

        /// <summary>
        ///     操作符号
        /// </summary>
        /// <param name="bexp"> 操作符号 </param>
        /// <param name="left"> 操作符左边的SQL </param>
        /// <param name="right"> 操作符右边的SQL </param>
        protected virtual void VisitOperate(BinaryExpression bexp, string left, string right)
        {
            SqlList.Push(item: $"({left} {VisitOperate(bexp: bexp, left: left)} {right})");
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression cexp)
        {
            base.VisitConstant(cexp: cexp);
            if (cexp == null) return null;

            //  查找组中是否存在已有的参数，有则直接取出
            if (CurrentField.Value != null)
            {
                switch (CurrentField.Value.Field.StorageType)
                {
                    case EumStorageType.Json:
                        CurrentDbParameter = DbProvider.DbParam.Create(CurrentField.Value.Field.Name, parameterName: $"p{ParamList.Count}_{CurrentField.Value.Field.Name}", value: JsonConvert.SerializeObject(cexp.Value), type: DbType.String, output: false, len: CurrentField.Value.Field.FieldLength);
                        break;
                    case EumStorageType.Array:
                        if (cexp.Value is not IEnumerable lst) throw new DataException($"数据库字段设为EumStorageType.Array类型，但当前字段的类型不是有效的IEnumerable类型");
                        CurrentDbParameter = DbProvider.DbParam.Create(CurrentField.Value.Field.Name, parameterName: $"p{ParamList.Count}_{CurrentField.Value.Field.Name}", value: lst.ToString(","), type: DbType.String, output: false, len: CurrentField.Value.Field.FieldLength);
                        break;
                    default:
                        if (CurrentField.Value.Field.DbType != DbType.Object)
                        {
                            CurrentDbParameter = DbProvider.DbParam.Create(CurrentField.Value.Field.Name, parameterName: $"p{ParamList.Count}_{CurrentField.Value.Field.Name}", value: cexp.Value, type: CurrentField.Value.Field.DbType, output: false, len: CurrentField.Value.Field.FieldLength);
                        }
                        break;
                }
            }
            
            // 手动指定类型
            CurrentDbParameter ??= DbProvider.DbParam.Create(CurrentFieldName, parameterName: $"p{ParamList.Count}_{CurrentFieldName}", value: cexp.Value, valType: cexp.Type, output: false, len: CurrentField.Value?.Field.FieldLength ?? 0);

            ParamList.Add(item: CurrentDbParameter);
            PushParamName(CurrentDbParameter.ParameterName);
            CurrentFieldName = null;
            return cexp;
        }

        /// <summary>
        ///     将属性变量转换成T-SQL字段名
        /// </summary>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return null;

            CurrentField = SetMap.PhysicsMap.GetState(propertyName: m.Member.Name);
            // 为null，说明当前不是属性字段，而是属性字段的属性，如.Count、.Length等
            if (CurrentField.Key == null)
            {
                if (m.Member.Name is "Count" or "Length")
                {
                    VisitMemberAccess(m: (MemberExpression)m.Expression);
                    PushFieldName(FunctionProvider.Len(SqlList.Pop()));
                    return m;
                }

                // 子属性不是Count、length时，说明当前属性并未识别，先简单过滤掉。直接获取上一级属性（暂时不知道哪些时候会发生这个情况）
                if (m.Expression != null && m.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    VisitMemberAccess(m: (MemberExpression)m.Expression);
                    return m;
                }

                // 上一级属性并不是属性时，只能直接把当前的Member.Name作为数据库字段名称了。（暂时不知道哪些时候会发生这个情况）
                CurrentFieldName = m.Member.Name;
                PushFieldName(DbProvider.KeywordAegis(CurrentFieldName));
                return m;
            }

            // 加入Sql队列
            CurrentFieldName = CurrentField.Value.Field.Name;
            VisitMemberAccess(keyValue: CurrentField);
            return m;
        }

        /// <summary>
        ///     加入字段到队列中
        /// </summary>
        /// <param name="keyValue"> 当前字段属性 </param>
        protected virtual void VisitMemberAccess(KeyValuePair<PropertyInfo, DbFieldMapState> keyValue)
        {
            PushFieldName(keyValue.Value.Field.IsFun ? keyValue.Value.Field.Name : DbProvider.KeywordAegis(fieldName: keyValue.Value.Field.Name));
        }

        /// <summary>
        ///     值类型的转换
        /// </summary>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Not) IsNot = true;

            return base.VisitUnary(u: u);
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


            // 如果m.Object能压缩，证明不是字段（必须先解析字段，再解析值）
            var result = IsFieldValue(exp: methodObject);
            if (!result) Visit(exp: methodObject);

            foreach (var t in arguments) Visit(exp: t);

            if (result) Visit(exp: methodObject);

            return m;
        }

        /// <summary>
        ///     当存在true 时，特殊处理
        /// </summary>
        protected virtual string SqlTrue(string sql)
        {
            var dbParam = ParamList.FirstOrDefault(predicate: o => o.ParameterName == sql);
            if (dbParam != null)
            {
                var result = dbParam.Value.ToString().Equals(value: "true");
                ParamList.RemoveAll(match: o => o.ParameterName == sql);
                return result ? "1=1" : "1<>1";
            }

            return sql;
        }

        /// <summary>
        ///     忽略字段的方法
        /// </summary>
        protected virtual bool IsIgnoreMethod(MethodCallExpression m)
        {
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
        
        /// <summary>
        /// 将字段名称加入到列表
        /// </summary>
        protected void PushFieldName(string fieldName)
        {
            SqlList.Push(fieldName);
        }
        
        /// <summary>
        /// 将参数名称加入到列表
        /// </summary>
        protected void PushParamName(string paramName)
        {
            SqlList.Push(paramName);
        }
    }
}