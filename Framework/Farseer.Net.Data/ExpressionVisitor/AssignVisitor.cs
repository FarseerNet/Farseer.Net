using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Data.Client;
using FS.Data.Map;

namespace FS.Data.ExpressionVisitor
{
    /// <summary>
    ///     提供字段赋值时表达式树的解析
    /// </summary>
    public class AssignVisitor : AbsSqlVisitor
    {
        /// <summary>
        ///     提供字段赋值时表达式树的解析
        /// </summary>
        /// <param name="dbProvider"> 数据库提供者（不同数据库的特性） </param>
        /// <param name="map"> 字段映射 </param>
        /// <param name="paramList"> SQL参数列表 </param>
        public AssignVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider: dbProvider, map: map, paramList: paramList)
        {
        }

        public new string Visit(Expression exp)
        {
            base.Visit(exp: exp);
            var sb = new StringBuilder();
            SqlList.Reverse().ToList().ForEach(action: o => sb.Append(value: o + ","));
            return sb.Length > 0 ? sb.Remove(startIndex: sb.Length - 1, length: 1).ToString() : sb.ToString();
        }

        /// <summary>
        ///     操作符号
        /// </summary>
        /// <param name="bexp"> 操作符号 </param>
        /// <param name="left"> 操作符左边的SQL </param>
        /// <param name="right"> 操作符右边的SQL </param>
        protected override void VisitOperate(BinaryExpression bexp, string left, string right)
        {
            SqlList.Push(item: $"{left} {VisitOperate(bexp: bexp, left: left)} {right}");
        }

        /// <summary>
        ///     解析参数类型
        /// </summary>
        /// <param name="p"> </param>
        /// <returns> </returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            // 时间变量类型，给与当前时间
            if (p.Type == typeof(DateTime)) return VisitConstant(cexp: Expression.Constant(value: DateTime.Now));
            return p;
        }
    }
}