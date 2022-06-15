using System;
using System.Reflection;
using Farseer.Net.AspNetCore.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Farseer.Net.AspNetCore.DynamicApi
{
    /// <summary>
    /// 判断当前的Type是否为Controller
    /// </summary>
    public class DefaultControllerFeatureProvider : ControllerFeatureProvider
    {
        // 增加UseApiAttribute的判断，如果使用了UseApiAttribute，则认为是控制器
        protected override bool IsController(TypeInfo typeInfo) => IsControllerClass(typeInfo);

        public static bool IsControllerClass(TypeInfo typeInfo)
        {
            return typeInfo.IsClass
                   && !typeInfo.IsAbstract
                   && typeInfo.IsPublic
                   && !typeInfo.ContainsGenericParameters
                   && !typeInfo.IsDefined(typeof(NonControllerAttribute))
                   && typeInfo.IsDefined(typeof(UseApiAttribute));
        }
        
        public static bool IsAction(Type typeInfo)
        {
            return typeInfo.IsClass
                   && !typeInfo.IsAbstract
                   && typeInfo.IsPublic
                   && !typeInfo.ContainsGenericParameters
                   && !typeInfo.IsDefined(typeof(NonControllerAttribute))
                   && typeInfo.IsDefined(typeof(UseApiAttribute));
        }
    }
}