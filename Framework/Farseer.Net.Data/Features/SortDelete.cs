using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Farseer.Net.Cache;

namespace Farseer.Net.Data.Features
{
    /// <summary>
    ///     设置表、视图的软删除标记
    /// </summary>
    public sealed class SortDelete
    {
        /// <summary>
        ///     软删除标记字段名称
        /// </summary>
        public string Name { private get; set; }

        /// <summary>
        ///     数据库字段类型
        /// </summary>
        public eumSortDeleteType FieldType { private get; set; }

        /// <summary>
        ///     标记值
        /// </summary>
        public object Value { private get; set; }

        /// <summary>
        ///     初始化逻辑删除
        /// </summary>
        /// <param name="entityType">实体类型</param>
        public void Init(Type entityType)
        {
            Type fieldType;
            switch (FieldType)
            {
                case eumSortDeleteType.Number:
                    fieldType = Value.GetType();
                    break;
                case eumSortDeleteType.DateTime:
                    fieldType = typeof(DateTime);
                    Value = DateTime.Now;
                    break;
                default:
                    fieldType = typeof(bool);
                    break;
            }

            var dic = new Dictionary<string, Type> { [Name] = fieldType };

            // 如果当前类已包含该字段，则不创新派生类
            var sortDeleteClassType = entityType.GetProperty(Name) != null ? entityType : DynamicsClassTypeCacheManger.Cache(dic, entityType);
            var param = Expression.Parameter(sortDeleteClassType, "o");
            var member = Expression.MakeMemberAccess(param, sortDeleteClassType.GetMember(Name)[0]);

            // 时间类型字段，则以参数形式（解析时动态转为当前时间）
            if (FieldType == eumSortDeleteType.DateTime)
            {
                AssignExpression = Expression.Assign(member, Expression.Parameter(fieldType)); //DateTime.Now 
                CondictionExpression = Expression.Equal(member, Expression.Convert(Expression.Constant(null), fieldType));
            }
            else
            {
                var valConstant = Expression.Constant(Value, member.Type);
                AssignExpression = Expression.Assign(member, valConstant);
                CondictionExpression = Expression.NotEqual(member, valConstant);
            }
            CondictionExpression = Expression.Lambda(CondictionExpression, param);
        }

        /// <summary>
        ///     生成的用于条件查询时，过滤已删除标记的数据
        /// </summary>
        internal Expression CondictionExpression { get; private set; }

        /// <summary>
        ///     用于删除数据时，转换为Update赋值方式
        /// </summary>
        internal Expression AssignExpression { get; private set; }
    }
}