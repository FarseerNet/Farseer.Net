using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Collections.Pooled;
using FS.Cache;

namespace FS.Data.Features
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
        public EumSortDeleteType FieldType { private get; set; }

        /// <summary>
        ///     标记值
        /// </summary>
        public object Value { private get; set; }

        /// <summary>
        ///     生成的用于条件查询时，过滤已删除标记的数据
        /// </summary>
        internal Expression CondictionExpression { get; private set; }

        /// <summary>
        ///     用于删除数据时，转换为Update赋值方式
        /// </summary>
        internal Expression AssignExpression { get; private set; }

        /// <summary>
        ///     初始化逻辑删除
        /// </summary>
        /// <param name="entityType"> 实体类型 </param>
        public void Init(Type entityType)
        {
            Type fieldType;
            switch (FieldType)
            {
                case EumSortDeleteType.Number:
                    fieldType = Value.GetType();
                    break;
                case EumSortDeleteType.DateTime:
                    fieldType = typeof(DateTime);
                    Value     = DateTime.Now;
                    break;
                default:
                    fieldType = typeof(bool);
                    break;
            }

            using var dic = new PooledDictionary<string, Type> { [key: Name] = fieldType };

            // 如果当前类已包含该字段，则不创新派生类
            var sortDeleteClassType = entityType.GetProperty(name: Name) != null ? entityType : DynamicsClassTypeCacheManger.Cache(addProperty: dic, baseType: entityType);
            var param               = Expression.Parameter(type: sortDeleteClassType, name: "o");
            var member              = Expression.MakeMemberAccess(expression: param, member: sortDeleteClassType.GetMember(name: Name)[0]);

            // 时间类型字段，则以参数形式（解析时动态转为当前时间）
            if (FieldType == EumSortDeleteType.DateTime)
            {
                AssignExpression     = Expression.Assign(left: member, right: Expression.Parameter(type: fieldType)); //DateTime.Now 
                CondictionExpression = Expression.Equal(left: member, right: Expression.Convert(expression: Expression.Constant(value: null), type: fieldType));
            }
            else
            {
                var valConstant = Expression.Constant(value: Value, type: member.Type);
                AssignExpression     = Expression.Assign(left: member, right: valConstant);
                CondictionExpression = Expression.NotEqual(left: member, right: valConstant);
            }

            CondictionExpression = Expression.Lambda(body: CondictionExpression, param);
        }
    }
}