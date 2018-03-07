// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-02 13:29
// ********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using Farseer.Net.Core.Mapping;
using Farseer.Net.Core.Net;

namespace Farseer.Net.Core
{
    /// <summary>
    /// 对实体类的验证
    /// </summary>
    public class EntityValidator
    {
        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TEntity>(TEntity info, Action<Dictionary<string, List<string>>> tip)
        {
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = Check(info, out dicError);

            if (!result) { tip(dicError); }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError">返回错误消息,key：属性名称；vakue：错误消息</param>
        /// <param name="entity">要检测的实体</param>
        public static bool Check(object entity, out Dictionary<string, List<string>> dicError)
        {
            dicError = new Dictionary<string, List<string>>();
            if (entity == null) { dicError["传入的对象不能为null"] = new List<string> { "传入的实体对象entity，为null。" }; return false; }

            var entityType = entity.GetType();

            // 单个对象类型
            if (!entityType.IsGenericType || entityType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return CheckEntity(entity, out dicError); }

            // 集合类型
            var lst = entity as IEnumerable;
            if (lst == null)
            {
                dicError["该对象类型不支持"] = new List<string> { "暂不支持该对象类型的判断，需要添加对该对象的支持" }; return false;
            }
            foreach (var obj in lst)
            {
                if (!Check(obj, out dicError)) { return false; }
            }
            return true;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError">返回错误消息,key：属性名称；vakue：错误消息</param>
        /// <param name="entity">要检测的实体</param>
        private static bool CheckEntity(object entity, out Dictionary<string, List<string>> dicError)
        {
            dicError = new Dictionary<string, List<string>>();
            if (entity == null) { dicError["传入的对象不对为null"] = new List<string> { "传入的实体对象entity，为null。" }; return false; }

            var map = EntityPhysicsMap.Map(entity.GetType());
            foreach (var kic in map.MapList)
            {
                var propertyType = kic.Key.GetType();
                if (!kic.Key.CanRead) { continue; }

                var value = kic.Key.GetValue(entity);

                // 集合类型
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() != typeof(Nullable<>)) return Check(value, out dicError);

                var lstError = new List<string>();
                foreach (var validationAttribute in kic.Value.ValidationList)
                {
                    if (!validationAttribute.IsValid(value)) { lstError.Add(validationAttribute.ErrorMessage); }
                }

                if (lstError.Count > 0) { dicError.Add(kic.Key.Name, lstError); }
            }
            return dicError.Count == 0;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static ApiResponseJson Check<TEntity>(TEntity info)
        {
            // 返回错误
            Dictionary<string, List<string>> dicError;
            var json = Check(info, out dicError) ? ApiResponseJson.Success("实体类检测通过") : ApiResponseJson.Error("实体类检测失败");

            if (!json.Status)
            {
                foreach (var errs in dicError)
                {
                    json.StatusMessage += $"属性：{errs.Key}，出错了：";
                    for (var i = 0; i < errs.Value.Count; i++) { json.StatusMessage += $"{i + 1}、{errs.Value[i]}；"; }
                    json.StatusMessage += $" ";
                }
            }
            return json;
        }
    }
}