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
        /// <param name="entity">要添加子元素的值</param>
        public static void AddItem(object entity)
        {
            foreach (var fieldEntity in entity.GetType().GetFields()) { AddItem(fieldEntity, entity); }
            foreach (var property in entity.GetType().GetProperties()) { AddItem(property, entity); }
        }

        /// <summary>
        ///     动态添加List元素
        /// </summary>
        /// <param name="fieldInfo">字段类型</param>
        /// <param name="entity">所属实体变量</param>
        /// <param name="methodName">方法名称，默认是Add</param>
        private static void AddItem(FieldInfo fieldInfo, object entity, string methodName = "Add")
        {
            // 非泛型，退出执行
            if (!fieldInfo.FieldType.IsGenericType) { return; }

            var lstVal = FieldGetCacheManger.Cache(fieldInfo, entity);
            // 空时，反射创建
            if (lstVal == null)
            {
                lstVal = InstanceCacheManger.Cache(fieldInfo.FieldType);
                FieldSetCacheManger.Cache(fieldInfo, entity, lstVal);
            }

            // 获取执行方法
            var method = fieldInfo.FieldType.GetMethod(methodName);
            if (method == null) { return; }

            var lstParamInstance = new List<object>();
            foreach (var parameterInfo in method.GetParameters())
            {
                // 反射创建子元素
                object item;
                if (parameterInfo.ParameterType == typeof (string)) { item = string.Empty; }
                else
                { item = parameterInfo.ParameterType.IsClass ? InstanceCacheManger.Cache(parameterInfo.ParameterType) : 0; }
                lstParamInstance.Add(item);
            }
            method.Invoke(lstVal, lstParamInstance.ToArray());

            // 添加子元素到List中
            foreach (var field in lstVal.GetType().GetFields()) { AddItem(field, lstVal); }
            foreach (var property in lstVal.GetType().GetProperties()) { AddItem(property, lstVal); }
        }

        /// <summary>
        ///     动态添加List元素
        /// </summary>
        /// <param name="propertyInfo">字段类型</param>
        /// <param name="entity">所属实体变量</param>
        /// <param name="methodName">方法名称，默认是Add</param>
        private static void AddItem(PropertyInfo propertyInfo, object entity, string methodName = "Add")
        {
            // 非泛型，退出执行
            if (!propertyInfo.PropertyType.IsGenericType) { return; }
            var lstVal = PropertyGetCacheManger.Cache(propertyInfo, entity);
            // 空时，反射创建
            if (lstVal == null)
            {
                lstVal = InstanceCacheManger.Cache(propertyInfo.PropertyType);
                PropertySetCacheManger.Cache(propertyInfo, entity, lstVal);
            }

            // 获取执行方法
            var method = propertyInfo.PropertyType.GetMethod(methodName);
            if (method == null) { return; }

            var lstParamInstance = new List<object>();
            foreach (var parameterInfo in method.GetParameters())
            {
                // 反射创建子元素
                object item;
                if (parameterInfo.ParameterType == typeof (string)) { item = string.Empty; }
                else
                { item = parameterInfo.ParameterType.IsClass ? InstanceCacheManger.Cache(parameterInfo.ParameterType) : 0; }
                lstParamInstance.Add(item);
            }
            method.Invoke(lstVal, lstParamInstance.ToArray());
            foreach (var field in lstVal.GetType().GetFields()) { AddItem(field, lstVal); }
            foreach (var property in lstVal.GetType().GetProperties()) { AddItem(property, lstVal); }
        }

        /// <summary>
        ///     返回基本类型的数据
        /// </summary>
        /// <param name="argumType"></param>
        /// <returns></returns>
        public static object FillRandData(Type argumType)
        {
            var rand = new Random(DateTime.Now.Second);

            var val = new object();
            // 对   List 类型处理
            if (argumType.IsGenericType && argumType.GetGenericTypeDefinition() == typeof (Nullable<>)) { return FillRandData(argumType.GetGenericArguments()[0]); }
            switch (Type.GetTypeCode(argumType))
            {
                case TypeCode.Boolean:
                    val = rand.Next(0, 1) == 0;
                    break;
                case TypeCode.DateTime:
                    val = new DateTime(rand.Next(2000, DateTime.Now.Year), rand.Next(1, 12), rand.Next(1, 30), rand.Next(0, 23), rand.Next(0, 59), rand.Next(0, 59));
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
                    val = rand.Next(255);
                    break;
                default:
                    val = "测试数据";
                    break;
            }
            return ConvertHelper.ConvertType(val, argumType);
        }
    }
}