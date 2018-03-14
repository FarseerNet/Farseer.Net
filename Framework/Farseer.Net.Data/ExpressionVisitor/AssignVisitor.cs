using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Data.Infrastructure;
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
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public AssignVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        public new string Visit(Expression exp)
        {
            base.Visit(exp);
            var sb = new StringBuilder();
            SqlList.Reverse().ToList().ForEach(o => sb.Append(o + ","));
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }

        /// <summary>
        ///     操作符号
        /// </summary>
        /// <param name="bexp">操作符号</param>
        /// <param name="left">操作符左边的SQL</param>
        /// <param name="right">操作符右边的SQL</param>
        protected override void VisitOperate(BinaryExpression bexp, string left, string right)
        {
            SqlList.Push($"{left} {VisitOperate(bexp, left)} {right}");
        }

        /// <summary>
        ///     解析参数类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            // 时间变量类型，给与当前时间
            if (p.Type == typeof(DateTime)) { return VisitConstant(Expression.Constant(DateTime.Now)); }
            return p;
        }
    }
}