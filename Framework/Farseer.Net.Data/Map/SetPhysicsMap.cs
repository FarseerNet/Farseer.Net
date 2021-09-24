using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using FS.Core.Mapping;
using FS.Core.Mapping.Attribute;
using FS.Data.Cache;
using FS.Data.Internal;

namespace FS.Data.Map
{
    /// <summary>
    ///     字段 映射关系
    /// </summary>
    public class SetPhysicsMap
    {
        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, DbFieldMapState> MapList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type"> 实体类Type </param>
        internal SetPhysicsMap(Type type)
        {
            Type          = type;
            MapList       = new Dictionary<PropertyInfo, DbFieldMapState>();
            PrimaryFields = new Dictionary<PropertyInfo, FieldAttribute>();
            // 循环Set的字段
            foreach (var propertyInfo in Type.GetProperties())
            {
                #region 获取字段已标记的特性

                var modelAtt = new DbFieldMapState();
                var attrs    = propertyInfo.GetCustomAttributes(inherit: false);
                foreach (var item in attrs)
                {
                    // 字段
                    if (item is FieldAttribute fieldAtt)
                    {
                        modelAtt.Field = fieldAtt;
                        continue;
                    }

                    // 字符串长度
                    if (item is StringLengthAttribute stringLengthAtt)
                    {
                        modelAtt.StringLength = stringLengthAtt;
                        continue;
                    }

                    // 是否必填
                    if (item is RequiredAttribute requiredAtt)
                    {
                        modelAtt.Required = requiredAtt;
                        continue;
                    }

                    // 字段描述
                    if (item is DisplayAttribute displayAtt)
                    {
                        modelAtt.Display = displayAtt;
                        continue;
                    }

                    // 值的长度
                    if (item is RangeAttribute rangeAtt)
                    {
                        modelAtt.Range = rangeAtt;
                        continue;
                    }

                    // 正则
                    if (item is RegularExpressionAttribute regularExpressionAtt) modelAtt.RegularExpression = regularExpressionAtt;
                }

                #endregion

                #region 初始化字段映射

                if (modelAtt.Field == null) modelAtt.Field = new FieldAttribute { Name = propertyInfo.Name };

                if (string.IsNullOrEmpty(value: modelAtt.Field.Name)) modelAtt.Field.Name = propertyInfo.Name;

                if (modelAtt.Field.IsMap)
                {
                    // 主键
                    if (modelAtt.Field.IsPrimaryKey) PrimaryFields[key: propertyInfo] = modelAtt.Field;

                    // 标识
                    if (modelAtt.Field.IsDbGenerated) DbGeneratedFields = new KeyValuePair<PropertyInfo, FieldAttribute>(key: propertyInfo, value: modelAtt.Field);
                }

                #endregion

                #region 初始化字段描述映射

                if (modelAtt.Display == null) modelAtt.Display = new DisplayAttribute { Name = propertyInfo.Name };

                if (string.IsNullOrEmpty(value: modelAtt.Display.Name)) modelAtt.Display.Name = propertyInfo.Name;

                #endregion

                #region 加入智能错误显示消息

                // 是否必填
                if (modelAtt.Required != null && string.IsNullOrEmpty(value: modelAtt.Required.ErrorMessage)) modelAtt.Required.ErrorMessage = $"{modelAtt.Display.Name}，不能为空！";

                // 字符串长度判断
                if (modelAtt.StringLength != null && string.IsNullOrEmpty(value: modelAtt.StringLength.ErrorMessage))
                {
                    if (modelAtt.StringLength.MinimumLength > 0 && modelAtt.StringLength.MaximumLength > 0)
                        modelAtt.StringLength.ErrorMessage = $"{modelAtt.Display.Name}，长度范围必须为：{modelAtt.StringLength.MinimumLength} - {modelAtt.StringLength.MaximumLength} 个字符之间！";
                    else if (modelAtt.StringLength.MaximumLength > 0)
                        modelAtt.StringLength.ErrorMessage = $"{modelAtt.Display.Name}，长度不能大于{modelAtt.StringLength.MaximumLength}个字符！";
                    else
                        modelAtt.StringLength.ErrorMessage = $"{modelAtt.Display.Name}，长度不能小于{modelAtt.StringLength.MinimumLength}个字符！";
                }

                // 值的长度
                if (modelAtt.Range != null && string.IsNullOrEmpty(value: modelAtt.Range.ErrorMessage))
                {
                    decimal minnum;
                    decimal.TryParse(s: modelAtt.Range.Minimum.ToString(), result: out minnum);
                    decimal maximum;
                    decimal.TryParse(s: modelAtt.Range.Minimum.ToString(), result: out maximum);

                    if (minnum > 0 && maximum > 0)
                        modelAtt.Range.ErrorMessage = $"{modelAtt.Display.Name}，的值范围必须为：{minnum} - {maximum} 之间！";
                    else if (maximum > 0)
                        modelAtt.Range.ErrorMessage = $"{modelAtt.Display.Name}，的值不能大于{maximum}！";
                    else
                        modelAtt.Range.ErrorMessage = $"{modelAtt.Display.Name}，的值不能小于{minnum}！";
                }

                #endregion

                MapList.Add(key: propertyInfo, value: modelAtt);
            }
        }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        internal InternalContext InternalContext { get; set; }

        /// <summary>
        ///     设置了主键的字段列表
        /// </summary>
        public Dictionary<PropertyInfo, FieldAttribute> PrimaryFields { get; }

        /// <summary>
        ///     设置了标识的字段
        /// </summary>
        public KeyValuePair<PropertyInfo, FieldAttribute> DbGeneratedFields { get; }

        /// <summary>
        ///     类型
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator SetPhysicsMap(Type type) => SetMapCacheManger.Cache(key: type);

        /// <summary>
        ///     获取当前属性（通过使用的fieldName）
        /// </summary>
        /// <param name="propertyName"> 属性名称 </param>
        public KeyValuePair<PropertyInfo, DbFieldMapState> GetState(string propertyName) => string.IsNullOrEmpty(value: propertyName) ? MapList.FirstOrDefault(predicate: o => o.Value.Field.IsPrimaryKey) : MapList.FirstOrDefault(predicate: o => o.Key.Name == propertyName);

        /// <summary>
        ///     获取当前属性（通过使用的fieldName）
        /// </summary>
        /// <param name="fieldName"> 属性名称 </param>
        public KeyValuePair<PropertyInfo, DbFieldMapState> GetStateByFieldName(string fieldName) => MapList.FirstOrDefault(predicate: o => o.Value.Field.Name == fieldName);
    }
}