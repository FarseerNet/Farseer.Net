using System;
using System.Collections.Generic;
using System.Reflection;
using FS.Cache;

namespace FS.Utils.Common
{
    /// <summary>
    ///     动态操作
    /// </summary>
    public static class DynamicOperate
    {
        /// <summary>
        ///     为entity的属性、字段 添加子元素。（属性、字段为List类型时）
        /// </summary>
        /// <param name="entity"> 要添加子元素的值 </param>
        public static void AddItem(object entity)
        {
            foreach (var fieldEntity in entity.GetType().GetFields()) AddItem(fieldInfo: fieldEntity, entity: entity);
            foreach (var property in entity.GetType().GetProperties()) AddItem(propertyInfo: property, entity: entity);
        }

        /// <summary>
        ///     动态添加List元素
        /// </summary>
        /// <param name="fieldInfo"> 字段类型 </param>
        /// <param name="entity"> 所属实体变量 </param>
        /// <param name="methodName"> 方法名称，默认是Add </param>
        private static void AddItem(FieldInfo fieldInfo, object entity, string methodName = "Add")
        {
            // 非泛型，退出执行
            if (!fieldInfo.FieldType.IsGenericType) return;

            var lstVal = FieldGetCacheManger.Cache(key: fieldInfo, instance: entity);
            // 空时，反射创建
            if (lstVal == null)
            {
                lstVal = InstanceCacheManger.Cache(type: fieldInfo.FieldType);
                FieldSetCacheManger.Cache(key: fieldInfo, instance: entity, value: lstVal);
            }

            // 获取执行方法
            var method = fieldInfo.FieldType.GetMethod(name: methodName);
            if (method == null) return;

            var lstParamInstance = new List<object>();
            foreach (var parameterInfo in method.GetParameters())
            {
                // 反射创建子元素
                object item;
                if (parameterInfo.ParameterType == typeof(string))
                    item = string.Empty;
                else
                    item = parameterInfo.ParameterType.IsClass ? InstanceCacheManger.Cache(type: parameterInfo.ParameterType) : 0;
                lstParamInstance.Add(item: item);
            }

            method.Invoke(obj: lstVal, parameters: lstParamInstance.ToArray());

            // 添加子元素到List中
            foreach (var field in lstVal.GetType().GetFields()) AddItem(fieldInfo: field, entity: lstVal);
            foreach (var property in lstVal.GetType().GetProperties()) AddItem(propertyInfo: property, entity: lstVal);
        }

        /// <summary>
        ///     动态添加List元素
        /// </summary>
        /// <param name="propertyInfo"> 字段类型 </param>
        /// <param name="entity"> 所属实体变量 </param>
        /// <param name="methodName"> 方法名称，默认是Add </param>
        private static void AddItem(PropertyInfo propertyInfo, object entity, string methodName = "Add")
        {
            // 非泛型，退出执行
            if (!propertyInfo.PropertyType.IsGenericType) return;
            var lstVal = PropertyGetCacheManger.Cache(key: propertyInfo, instance: entity);
            // 空时，反射创建
            if (lstVal == null)
            {
                lstVal = InstanceCacheManger.Cache(type: propertyInfo.PropertyType);
                PropertySetCacheManger.Cache(key: propertyInfo, instance: entity, value: lstVal);
            }

            // 获取执行方法
            var method = propertyInfo.PropertyType.GetMethod(name: methodName);
            if (method == null) return;

            var lstParamInstance = new List<object>();
            foreach (var parameterInfo in method.GetParameters())
            {
                // 反射创建子元素
                object item;
                if (parameterInfo.ParameterType == typeof(string))
                    item = string.Empty;
                else
                    item = parameterInfo.ParameterType.IsClass ? InstanceCacheManger.Cache(type: parameterInfo.ParameterType) : 0;
                lstParamInstance.Add(item: item);
            }

            method.Invoke(obj: lstVal, parameters: lstParamInstance.ToArray());
            foreach (var field in lstVal.GetType().GetFields()) AddItem(fieldInfo: field, entity: lstVal);
            foreach (var property in lstVal.GetType().GetProperties()) AddItem(propertyInfo: property, entity: lstVal);
        }

        /// <summary>
        ///     返回基本类型的数据
        /// </summary>
        /// <param name="argumType"> </param>
        /// <returns> </returns>
        public static object FillRandData(Type argumType)
        {
            var rand = new Random(Seed: DateTime.Now.Second);

            var val = new object();
            // 对   List 类型处理
            if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof(Nullable<>)) return FillRandData(argumType: argumType.GetGenericArguments()[0]);
            switch (Type.GetTypeCode(type: argumType))
            {
                case TypeCode.Boolean:
                    val = rand.Next(minValue: 0, maxValue: 1) == 0;
                    break;
                case TypeCode.DateTime:
                    val = new DateTime(year: rand.Next(minValue: 2000, maxValue: DateTime.Now.Year), month: rand.Next(minValue: 1, maxValue: 12), day: rand.Next(minValue: 1, maxValue: 30), hour: rand.Next(minValue: 0, maxValue: 23), minute: rand.Next(minValue: 0, maxValue: 59), second: rand.Next(minValue: 0, maxValue: 59));
                    break;
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    val = rand.Next(maxValue: 255);
                    break;
                default:
                    val = "测试数据";
                    break;
            }

            return ConvertHelper.ConvertType(sourceValue: val, returnType: argumType);
        }
    }
}