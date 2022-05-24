using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FS.Data.Client;
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
        /// <param name="dbProvider"> 数据库提供者（不同数据库的特性） </param>
        /// <param name="map"> 字段映射 </param>
        /// <param name="paramList"> SQL参数列表 </param>
        public WhereVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider: dbProvider, map: map, paramList: paramList)
        {
        }

        public new string Visit(Expression exp)
        {
            base.Visit(exp: exp);
            return string.Join(" AND ", SqlList.Reverse());
            //return IEnumerableHelper.ToString(lst: SqlList.Reverse(), sign: " AND ");
        }

        protected override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Arguments.Count == 0 && nex.Type.IsGenericType) VisitConstant(cexp: Expression.Constant(value: null));
            VisitExpressionList(original: nex.Arguments);
            return nex;
        }

        /// <summary>
        ///     解析方法
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            var exp = base.VisitMethodCall(m: m);

            if (ClearCallSql()) return m;
            if (IsIgnoreMethod(m: m)) return m;

            #region 字段、参数、值类型

            Type   fieldType = null;
            Type   paramType = null;
            string fieldName = null;
            string paramName = null;

            if (m.Arguments.Count > 0)
            {
                var methodObject = m.Object;
                var arguments    = m.Arguments.ToList();
                if (methodObject == null)
                {
                    methodObject = arguments[index: 0];
                    arguments.RemoveAt(index: 0);
                }

                // 非List array类型
                if (!IsGenericOrArray(type: methodObject.Type))
                {
                    fieldType = methodObject.Type;
                    if (CurrentDbParameter != null)
                    {
                        paramType = arguments[index: 0].Type;
                        paramName = SqlList.Pop();
                    }

                    fieldName = SqlList.Pop();
                }
                else
                {
                    paramType = methodObject.Type;
                    paramName = CurrentDbParameter != null ? SqlList.Pop() : null;
                    fieldType = CurrentDbParameter != null ? arguments[index: 0].Type : arguments[index: 1].Type;
                    fieldName = SqlList.Pop();
                }
            }

            #endregion

            switch (m.Method.Name)
            {
                case "Contains":
                    VisitMethodContains(m: m, fieldType: fieldType, fieldName: fieldName, paramType: paramType, paramName: paramName);
                    break;
                case "StartsWith":
                    VisitMethodStartsWith(fieldType: fieldType, fieldName: fieldName, paramType: paramType, paramName: paramName);
                    break;
                case "EndsWith":
                    VisitMethodEndsWith(fieldType: fieldType, fieldName: fieldName, paramType: paramType, paramName: paramName);
                    break;
                case "IsEquals":
                    VisitMethodIsEquals(fieldType: fieldType, fieldName: fieldName, paramType: paramType, paramName: paramName);
                    break;
                case "Equals":
                    VisitMethodEquals(fieldType: fieldType, fieldName: fieldName, paramType: paramType, paramName: paramName);
                    break;
                case "ToShortDate":
                    VisitMethodToShortDate(fieldType: fieldType, fieldName: fieldName);
                    break;
                default:
                {
                    if (m.Arguments.Count == 0 && m.Object != null) return m;
                    throw new Exception(message: string.Format(format: "暂不支持该方法的SQL转换：" + m.Method.Name.ToUpper()));
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
            if (ParamList != null && ParamList.Count > 0 && (ParamList.Last().Value == null || string.IsNullOrWhiteSpace(value: ParamList.Last().Value.ToString())))
            {
                CurrentDbParameter = null;
                ParamList.RemoveAt(index: ParamList.Count - 1);
                SqlList.Pop();
                SqlList.Pop();
                SqlList.Push(item: "1<>1");
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Contains方法解析
        /// </summary>
        /// <param name="fieldType"> </param>
        /// <param name="fieldName"> </param>
        /// <param name="paramType"> </param>
        /// <param name="paramName"> </param>
        protected virtual void VisitMethodContains(MethodCallExpression m, Type fieldType, string fieldName, Type paramType, string paramName)
        {
            // 非List<>形式
            if (paramType != null && !IsGenericOrArray(type: paramType))
            {
                #region 搜索值串的处理

                var param = ParamList.Find(match: o => o.ParameterName == paramName);
                if (param != null && Regex.IsMatch(input: param.Value.ToString(), pattern: @"[\d]+") && (Type.GetTypeCode(type: fieldType) == TypeCode.Int16 || Type.GetTypeCode(type: fieldType) == TypeCode.Int32 || Type.GetTypeCode(type: fieldType) == TypeCode.Decimal || Type.GetTypeCode(type: fieldType) == TypeCode.Double || Type.GetTypeCode(type: fieldType) == TypeCode.Int64 || Type.GetTypeCode(type: fieldType) == TypeCode.UInt16 || Type.GetTypeCode(type: fieldType) == TypeCode.UInt32 || Type.GetTypeCode(type: fieldType) == TypeCode.UInt64))
                {
                    param.Value  = "," + param.Value + ",";
                    param.DbType = DbType.String;
                    if (DbProvider.KeywordAegis(fieldName: "").Length > 0)
                        fieldName = "','+" + fieldName.Substring(startIndex: 1, length: fieldName.Length - 2) + "+','";
                    else
                        fieldName = "','+" + fieldName + "+','";
                }

                #endregion

                // 判断是不是字段调用Contains
                var isFieldCall = m.Object != null && m.Object.NodeType == ExpressionType.MemberAccess && ((MemberExpression)m.Object).Expression != null && ((MemberExpression)m.Object).Expression.NodeType == ExpressionType.Parameter;
                SqlList.Push(item: isFieldCall ? FunctionProvider.CharIndex(fieldName: fieldName, paramName: paramName, isNot: IsNot) : FunctionProvider.CharIndex(fieldName: paramName, paramName: fieldName, isNot: IsNot));
            }
            else
            {
                // 删除参数化，后面需要改成多参数
                var paramValue = CurrentDbParameter.Value.ToString();
                ParamList.RemoveAt(index: ParamList.Count - 1);

                // 参数类型，转换成多参数
                var lstParamName = new List<string>();
                var index        = 0;
                foreach (var val in paramValue.Split(','))
                {
                    var param = DbProvider.DbParam.Create(name: CurrentDbParameter.ParameterName.Substring(startIndex: 1) + "_" + index++, value: val, valType: fieldType);
                    lstParamName.Add(item: param.ParameterName);
                    ParamList.Add(item: param);
                }

                SqlList.Push(item: FunctionProvider.In(fieldName: fieldName, lstParamName: lstParamName, isNot: IsNot));
            }
        }

        /// <summary>
        ///     StartSwith方法解析
        /// </summary>
        /// <param name="fieldType"> </param>
        /// <param name="fieldName"> </param>
        /// <param name="paramType"> </param>
        /// <param name="paramName"> </param>
        protected virtual void VisitMethodStartsWith(Type fieldType, string fieldName, Type paramType, string paramName) => SqlList.Push(item: FunctionProvider.CharIndex(fieldName: fieldName, paramName: paramName, isNot: IsNot));

        /// <summary>
        ///     EndSwith方法解析
        /// </summary>
        /// <param name="fieldType"> </param>
        /// <param name="fieldName"> </param>
        /// <param name="paramType"> </param>
        /// <param name="paramName"> </param>
        protected virtual void VisitMethodEndsWith(Type fieldType, string fieldName, Type paramType, string paramName)
        {
            SqlList.Push(item: FunctionProvider.EndsWith(fieldName: fieldName, paramName: paramName, isNot: IsNot));
            CurrentDbParameter.Value = $"%{CurrentDbParameter.Value}";
        }

        /// <summary>
        ///     IsEquals方法解析
        /// </summary>
        /// <param name="fieldType"> </param>
        /// <param name="fieldName"> </param>
        /// <param name="paramType"> </param>
        /// <param name="paramName"> </param>
        protected virtual void VisitMethodIsEquals(Type fieldType, string fieldName, Type paramType, string paramName) => SqlList.Push(item: FunctionProvider.IsEquals(fieldName: fieldName, paramName: paramName, isNot: IsNot));

        /// <summary>
        ///     IsEquals方法解析
        /// </summary>
        /// <param name="fieldType"> </param>
        /// <param name="fieldName"> </param>
        /// <param name="paramType"> </param>
        /// <param name="paramName"> </param>
        protected virtual void VisitMethodEquals(Type fieldType, string fieldName, Type paramType, string paramName) => SqlList.Push(item: FunctionProvider.IsEquals(fieldName: fieldName, paramName: paramName, isNot: IsNot));

        /// <summary>
        ///     ToShortDate方法解析
        /// </summary>
        /// <param name="fieldType"> </param>
        /// <param name="fieldName"> </param>
        protected virtual void VisitMethodToShortDate(Type fieldType, string fieldName) => SqlList.Push(item: FunctionProvider.ToShortDate(fieldName: fieldName));

        /// <summary>
        ///     是否为数组或泛型类型
        /// </summary>
        /// <param name="type"> Type </param>
        public static bool IsGenericOrArray(Type type) => type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() != typeof(Nullable<>);
    }
}