using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FS.Core.Mapping;
using FS.ElasticSearch.Cache;

namespace FS.ElasticSearch.Map
{
    /// <summary>
    ///     字段 映射关系
    /// </summary>
    public class SetPhysicsMap
    {
        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, FieldMapState> MapList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        internal SetPhysicsMap(Type type)
        {
            Type = type;
            MapList = new Dictionary<PropertyInfo, FieldMapState>();

            // 循环Set的字段
            foreach (var propertyInfo in Type.GetProperties())
            {
                #region 获取字段已标记的特性

                var modelAtt = new FieldMapState();
                var attrs = propertyInfo.GetCustomAttributes(false);
                foreach (var item in attrs)
                {
                    // 字符串长度
                    if (item is StringLengthAttribute stringLengthAtt) { modelAtt.StringLength = stringLengthAtt; continue; }
                    // 是否必填
                    if (item is RequiredAttribute requiredAtt) { modelAtt.Required = requiredAtt; continue; }
                    // 字段描述
                    if (item is DisplayAttribute displayAtt) { modelAtt.Display = displayAtt; continue; }
                    // 值的长度
                    if (item is RangeAttribute rangeAtt) { modelAtt.Range = rangeAtt; continue; }
                    // 正则
                    if (item is RegularExpressionAttribute regularExpressionAtt) { modelAtt.RegularExpression = regularExpressionAtt; continue; }
                }
                #endregion

                #region 初始化字段描述映射

                modelAtt.Display ??= new DisplayAttribute {Name = propertyInfo.Name};
                if (string.IsNullOrEmpty(modelAtt.Display.Name)) { modelAtt.Display.Name = propertyInfo.Name; }

                #endregion

                #region 加入智能错误显示消息

                // 是否必填
                if (modelAtt.Required != null && string.IsNullOrEmpty(modelAtt.Required.ErrorMessage)) { modelAtt.Required.ErrorMessage = $"{modelAtt.Display.Name}，不能为空！"; }

                // 字符串长度判断
                if (modelAtt.StringLength != null && string.IsNullOrEmpty(modelAtt.StringLength.ErrorMessage))
                {
                    if (modelAtt.StringLength.MinimumLength > 0 && modelAtt.StringLength.MaximumLength > 0) { modelAtt.StringLength.ErrorMessage = $"{modelAtt.Display.Name}，长度范围必须为：{modelAtt.StringLength.MinimumLength} - {modelAtt.StringLength.MaximumLength} 个字符之间！"; }
                    else if (modelAtt.StringLength.MaximumLength > 0) { modelAtt.StringLength.ErrorMessage = $"{modelAtt.Display.Name}，长度不能大于{modelAtt.StringLength.MaximumLength}个字符！"; }
                    else
                    { modelAtt.StringLength.ErrorMessage = $"{modelAtt.Display.Name}，长度不能小于{modelAtt.StringLength.MinimumLength}个字符！"; }
                }

                // 值的长度
                if (modelAtt.Range != null && string.IsNullOrEmpty(modelAtt.Range.ErrorMessage))
                {
                    decimal.TryParse(modelAtt.Range.Minimum.ToString(), out var minnum);
                    decimal.TryParse(modelAtt.Range.Minimum.ToString(), out var maximum);

                    if (minnum > 0 && maximum > 0) { modelAtt.Range.ErrorMessage = $"{modelAtt.Display.Name}，的值范围必须为：{minnum} - {maximum} 之间！"; }
                    else if (maximum > 0) { modelAtt.Range.ErrorMessage = $"{modelAtt.Display.Name}，的值不能大于{maximum}！"; }
                    else
                    { modelAtt.Range.ErrorMessage = $"{modelAtt.Display.Name}，的值不能小于{minnum}！"; }
                }

                #endregion

                MapList.Add(propertyInfo, modelAtt);
            }
        }

        /// <summary>
        ///     类型
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator SetPhysicsMap(Type type) => SetMapCacheManger.Cache(type);
    }
}