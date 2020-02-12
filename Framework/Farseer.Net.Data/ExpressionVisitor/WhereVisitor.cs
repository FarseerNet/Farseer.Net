using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FS.Data.Infrastructure;
using FS.Data.Map;
using FS.Utils.Common;

namespace FS.Data.ExpressionVisitor
{
    /// <summary>
    ///     提供ExpressionBinary表达式树的解析
    /// </summary>
    public class WhereVisitor : AbsSqlVisitor
    {
        /// <summary>
        ///     Select筛选字段时表达式树的解析
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public WhereVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        public new string Visit(Expression exp)
        {
            base.Visit(exp);
            return IEnumerableHelper.ToString(SqlList.Reverse(), " AND ");
        }
        
        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Arguments.Count == 0 && nex.Type.IsGenericType) { VisitConstant(Expression.Constant(null)); }
            VisitExpressionList(nex.Arguments);
            return nex;
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            var exp = base.VisitMethodCall(m);

            if (ClearCallSql()) { return m; }
            if (IsIgnoreMethod(m)) { return m; }

            #region 字段、参数、值类型

            Type fieldType = null;
            Type paramType = null;
            string fieldName = null;
            string paramName = null;

            if (m.Arguments.Count > 0)
            {
                var methodObject = m.Object;
                var arguments = m.Arguments.ToList();
                if (methodObject == null)
                {
                    methodObject = arguments[0];
                    arguments.RemoveAt(0);
                }
                // 非List array类型
                if (!IsGenericOrArray(methodObject.Type))
                {
                    fieldType = methodObject.Type;
                    if (CurrentDbParameter != null)
                    {
                        paramType = arguments[0].Type;
                        paramName = SqlList.Pop();
                    }
                    fieldName = SqlList.Pop();
                }
                else
                {
                    paramType = methodObject.Type;
                    paramName = CurrentDbParameter != null ? SqlList.Pop() : null;
                    fieldType = CurrentDbParameter != null ? arguments[0].Type : arguments[1].Type;
                    fieldName = SqlList.Pop();
                }
            }

            #endregion

            switch (m.Method.Name)
            {
                case "Contains":
                    VisitMethodContains(m, fieldType, fieldName, paramType, paramName);
                    break;
                case "StartsWith":
                    VisitMethodStartswith(fieldType, fieldName, paramType, paramName);
                    break;
                case "EndsWith":
                    VisitMethodEndswith(fieldType, fieldName, paramType, paramName);
                    break;
                case "IsEquals":
                    VisitMethodIsEquals(fieldType, fieldName, paramType, paramName);
                    break;
                case "Equals":
                    VisitMethodEquals(fieldType, fieldName, paramType, paramName);
                    break;
                case "ToShortDate":
                    VisitMethodToShortDate(fieldType, fieldName);
                    break;
                default:
                    {
                        if (m.Arguments.Count == 0 && m.Object != null) { return m; }
                        throw new Exception(string.Format("暂不支持该方法的SQL转换：" + m.Method.Name.ToUpper()));
                    }
            }
            IsNot = false;
            return exp;
        }

        /// <summary>
        ///     清除值为空的条件，并给与1!=1的SQL
        /// </summary>
        private bool ClearCallSql()
        {
            if (ParamList != null && ParamList.Count > 0 && (ParamList.Last().Value == null || string.IsNullOrWhiteSpace(ParamList.Last().Value.ToString())))
            {
                CurrentDbParameter = null;
                ParamList.RemoveAt(ParamList.Count - 1);
                SqlList.Pop();
                SqlList.Pop();
                SqlList.Push("1<>1");
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Contains方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        protected virtual void VisitMethodContains(MethodCallExpression m, Type fieldType, string fieldName, Type paramType, string paramName)
        {
            // 非List<>形式
            if (paramType != null && !IsGenericOrArray(paramType))
            {
                #region 搜索值串的处理
                var param = ParamList.Find(o => o.ParameterName == paramName);
                if (param != null && Regex.IsMatch(param.Value.ToString(), @"[\d]+") && (Type.GetTypeCode(fieldType) == TypeCode.Int16 || Type.GetTypeCode(fieldType) == TypeCode.Int32 || Type.GetTypeCode(fieldType) == TypeCode.Decimal || Type.GetTypeCode(fieldType) == TypeCode.Double || Type.GetTypeCode(fieldType) == TypeCode.Int64 || Type.GetTypeCode(fieldType) == TypeCode.UInt16 || Type.GetTypeCode(fieldType) == TypeCode.UInt32 || Type.GetTypeCode(fieldType) == TypeCode.UInt64))
                {
                    param.Value = "," + param.Value + ",";
                    param.DbType = DbType.String;
                    if (DbProvider.KeywordAegis("").Length > 0) { fieldName = "','+" + fieldName.Substring(1, fieldName.Length - 2) + "+','"; }
                    else
                    { fieldName = "','+" + fieldName + "+','"; }
                }
                #endregion

                // 判断是不是字段调用Contains
                var isFieldCall = m.Object != null && m.Object.NodeType == ExpressionType.MemberAccess && ((MemberExpression)m.Object).Expression != null && ((MemberExpression)m.Object).Expression.NodeType == ExpressionType.Parameter;
                SqlList.Push(isFieldCall ? FunctionProvider.CharIndex(fieldName, paramName, IsNot) : FunctionProvider.CharIndex(paramName, fieldName, IsNot));
            }
            else
            {
                // 删除参数化，后面需要改成多参数
                var paramValue = CurrentDbParameter.Value.ToString();
                ParamList.RemoveAt(ParamList.Count - 1);

                // 参数类型，转换成多参数
                var lstParamName = new List<string>();
                var index = 0;
                foreach (var val in paramValue.Split(','))
                {
                    var param = DbProvider.CreateDbParam(CurrentDbParameter.ParameterName.Substring(1) + "_" + index++, val, fieldType);
                    lstParamName.Add(param.ParameterName);
                    ParamList.Add(param);
                }

                SqlList.Push(FunctionProvider.In(fieldName, lstParamName, IsNot));
            }
        }

        /// <summary>
        ///     StartSwith方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        protected virtual void VisitMethodStartswith(Type fieldType, string fieldName, Type paramType, string paramName) => SqlList.Push(FunctionProvider.CharIndex(fieldName, paramName, IsNot));

        /// <summary>
        ///     EndSwith方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        protected virtual void VisitMethodEndswith(Type fieldType, string fieldName, Type paramType, string paramName)
        {
            SqlList.Push(FunctionProvider.EndsWith(fieldName, paramName, IsNot));
            CurrentDbParameter.Value = $"%{CurrentDbParameter.Value}";
        }

        /// <summary>
        ///     IsEquals方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        protected virtual void VisitMethodIsEquals(Type fieldType, string fieldName, Type paramType, string paramName) => SqlList.Push(FunctionProvider.IsEquals(fieldName, paramName, IsNot));

        /// <summary>
        ///     IsEquals方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <param name="paramType"></param>
        /// <param name="paramName"></param>
        protected virtual void VisitMethodEquals(Type fieldType, string fieldName, Type paramType, string paramName) => SqlList.Push(FunctionProvider.IsEquals(fieldName, paramName, IsNot));

        /// <summary>
        ///     ToShortDate方法解析
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        protected virtual void VisitMethodToShortDate(Type fieldType, string fieldName) => SqlList.Push(FunctionProvider.ToShortDate(fieldName));

        /// <summary>
        /// 是否为数组或泛型类型
        /// </summary>
        /// <param name="type">Type</param>
        public static bool IsGenericOrArray(Type type) => type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() != typeof(Nullable<>));
    }
}