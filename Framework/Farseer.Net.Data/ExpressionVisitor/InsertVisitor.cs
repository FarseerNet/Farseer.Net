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
    ///     提供字段插入表达式树的解析
    /// </summary>
    public class InsertVisitor : AbsSqlVisitor
    {
        /// <summary>
        ///     提供字段插入表达式树的解析
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public InsertVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        public new string Visit(Expression exp)
        {
            base.Visit(exp);
            //  字段
            var strFields = new StringBuilder();
            //  值
            var strValues = new StringBuilder();

            var lst = SqlList.Reverse().ToList();
            for (var i = 0; i < lst.Count; i++)
            {
                //  添加参数到列表
                strFields.AppendFormat("{0},", lst[i]);
                strValues.AppendFormat("{0},", ParamList[i].ParameterName);
            }
            return "(" + strFields.Remove(strFields.Length - 1, 1) + ") VALUES (" + strValues.Remove(strValues.Length - 1, 1) + ")";
        }

        /// <summary>
        ///     将二元符号转换成T-SQL可识别的操作符
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression b)
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
            left = base.Visit(left);
            right = base.Visit(right);
            var conversion = base.Visit(b.Conversion);

            var contidion = isReverse ? (left != b.Right || right != b.Left) : (left != b.Left || right != b.Right);
            // 说明进过了换算
            if (contidion || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null) { return Expression.Coalesce(left, right, conversion as LambdaExpression); }
                else
                {
                    // 两边类型不同时，需要进行转换
                    if (left.Type != right.Type) { right = Expression.Convert(right, left.Type); }
                    b = Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
                }
            }

            // 清除状态（与或状态，不清除）
            if (b.NodeType != ExpressionType.And && b.NodeType != ExpressionType.Or)
            {
                CurrentFieldName = null;
                CurrentDbParameter = null;
            }
            return b;
        }

        /// <summary>
        ///     将属性变量的右边值，转换成T-SQL的字段值
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression cexp)
        {
            if (cexp == null) return null;

            //  查找组中是否存在已有的参数，有则直接取出
            CurrentDbParameter = DbProvider.CreateDbParam("p" + ParamList.Count + "_" + CurrentFieldName, cexp.Value, cexp.Type);
            ParamList.Add(CurrentDbParameter);
            CurrentFieldName = null;
            return cexp;
        }
    }
}