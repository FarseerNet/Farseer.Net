﻿// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-02 13:19
// ********************************************

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FS.Core.Mapping.Attribute;

namespace FS.Core.Mapping
{
    /// <summary>
    /// 保存普通实体属性映射的信息
    /// </summary>
    public class EntityPropertyMap
    {
        /// <summary>
        /// 验证特性列表
        /// </summary>
        public List<ValidationAttribute> ValidationList { get; set; }

        /// <summary>
        ///     属性中文描述
        /// </summary>
        public DisplayAttribute Display { get; set; }
    }
}