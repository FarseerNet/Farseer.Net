// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-02 17:38
// ********************************************

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Collections.Pooled;

namespace FS.Utils.Component
{
    /// <summary>
    ///     枚举帮助器
    /// </summary>
    public class Enums
    {
        /// <summary>
        ///     缓存类
        /// </summary>
        private static readonly PooledDictionary<string, string> CacheList = new();

        private readonly string _enumName;
        private readonly Enum   _eum;
        private readonly Type   _eumType;

        /// <summary>
        ///     枚举帮助器
        /// </summary>
        public Enums(Enum eum)
        {
            _eum      = eum;
            _eumType  = eum.GetType();
            _enumName = eum.ToString();
        }

        /// <summary>
        ///     获取枚举中文
        /// </summary>
        public string GetName()
        {
            var key = $"{_eumType.FullName}.{_eum.ToString()}";

            if (!CacheList.ContainsKey(key: key))
            {
                foreach (var fieldInfo in _eumType.GetFields())
                {
                    //判断名称是否相等   
                    if (fieldInfo.Name != _enumName) continue;

                    //反射出自定义属性   
                    foreach (Attribute attr in fieldInfo.GetCustomAttributes(inherit: true))
                    {
                        //类型转换找到一个Description，用Description作为成员名称
                        var dscript = attr as DisplayAttribute;
                        if (dscript == null) continue;
                        if (!CacheList.ContainsKey(key: key)) CacheList.Add(key: key, value: dscript.Name);
                        return dscript.Name;
                    }
                }

                // 还是找不到时，返回空
                return string.Empty;
            }

            return CacheList[key: key];
        }
    }
}