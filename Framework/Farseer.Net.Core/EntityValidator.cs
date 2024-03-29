﻿// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-02 13:29
// ********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using FS.Core.Mapping;
using FS.Core.Net;

namespace FS.Core
{
    /// <summary>
    ///     对实体类的验证
    /// </summary>
    public class EntityValidator
    {
        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TEntity>(TEntity info, Action<IDictionary<string, List<string>>> tip)
        {
            //返回错误
            var result = Check(entity: info, dicError: out var dicError);

            if (!result) tip(obj: dicError);
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static ApiResponseJson Check<TEntity>(TEntity info)
        {
            // 返回错误
            var json = Check(entity: info, dicError: out var dicError)
            ? ApiResponseJson.Success(statusMessage: "实体类检测通过")
            : ApiResponseJson.Error(statusMessage: "实体类检测失败");

            if (!json.Status)
            {
                foreach (var errs in dicError)
                {
                    json.StatusMessage += $"属性：{errs.Key}，出错了：";
                    for (var i = 0; i < errs.Value.Count; i++) json.StatusMessage += $"{i + 1}、{errs.Value[index: i]}；";
                    json.StatusMessage += " ";
                }
            }

            return json;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError"> 返回错误消息,key：属性名称；vakue：错误消息 </param>
        /// <param name="entity"> 要检测的实体 </param>
        public static bool Check(object entity, out IDictionary<string, List<string>> dicError)
        {
            dicError = new Dictionary<string, List<string>>();
            if (entity == null)
            {
                dicError[key: "传入的对象不能为null"] = new List<string> { "传入的实体对象entity，为null。" };
                return false;
            }

            var entityType = entity.GetType();

            // 单个对象类型
            if (!entityType.IsGenericType || entityType.GetGenericTypeDefinition() == typeof(Nullable<>)) return CheckEntity(entity: entity, dicError: out dicError);

            // 集合类型
            if (entity is IEnumerable lst)
            {
                foreach (var obj in lst)
                    if (!Check(entity: obj, dicError: out dicError))
                        return false;
            }

            return true;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError"> 返回错误消息,key：属性名称；vakue：错误消息 </param>
        /// <param name="entity"> 要检测的实体 </param>
        private static bool CheckEntity(object entity, out IDictionary<string, List<string>> dicError)
        {
            dicError = new Dictionary<string, List<string>>();
            if (entity == null)
            {
                dicError[key: "传入的对象不对为null"] = new List<string> { "传入的实体对象entity，为null。" };
                return false;
            }

            var map = EntityPhysicsMap.Map(type: entity.GetType());
            foreach (var kic in map.MapList)
            {
                if (!kic.Key.CanRead || !kic.Key.CanWrite) continue;
                var value = kic.Key.GetValue(obj: entity);

                // 集合类型
                if (value != null && kic.Key.PropertyType.IsGenericType && kic.Key.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) return Check(entity: value, dicError: out dicError);

                var lstError = new List<string>();
                foreach (var validationAttribute in kic.Value.ValidationList)
                    if (!validationAttribute.IsValid(value: value))
                        lstError.Add(item: validationAttribute.ErrorMessage);
                if (lstError.Count > 0) dicError.Add(key: kic.Key.Name, value: lstError);
            }

            return dicError.Count == 0;
        }
    }
}