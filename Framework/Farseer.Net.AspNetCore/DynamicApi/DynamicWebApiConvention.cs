using System;
using System.Linq;
using System.Reflection;
using FS.Core.Abstract.AspNetCore;
using FS.DI;
using FS.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Farseer.Net.AspNetCore.DynamicApi
{
    /// <summary>
    /// 定义应用层的路由约定
    /// </summary>
    public class DynamicWebApiConvention : IApplicationModelConvention
    {
        private readonly IDynamicWebApiOptions _dynamicWebApiOptions;
        public DynamicWebApiConvention()
        {
            this._dynamicWebApiOptions = IocManager.GetService<IDynamicWebApiOptions>();
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var type = controller.ControllerType.AsType();

                // 是否为应用服务层API
                if (DefaultControllerFeatureProvider.IsControllerClass(type.GetTypeInfo()))
                {
                    var useApiAttribute = type.GetCustomAttribute<UseApiAttribute>();
                    // 配置区域
                    ConfigureArea(controller, useApiAttribute);
                    ConfigureApiExplorer(controller);

                    var areaName = useApiAttribute.Area;

                    foreach (var actionModel in controller.Actions)
                    {
                        var apiAttribute = GetApiAttribute(actionModel);
                        SetVisible(actionModel, apiAttribute);
                        if (!actionModel.ApiExplorer.IsVisible.GetValueOrDefault()) continue;

                        // 添加HttpMethod
                        AddAppServiceSelector(areaName, actionModel, apiAttribute);

                        // 配置入参
                        ConfigureParameters(actionModel);
                    }
                }
            }
        }

        /// <summary>
        /// 设置Action可见度
        /// </summary>
        private bool SetVisible(ActionModel action, ApiAttribute attribute)
        {
            if (attribute == null)
            {
                action.ApiExplorer.IsVisible = false; // 对应的Api不映射
                return false;
            }
            action.ApiExplorer.IsVisible = true;
            return true;
        }

        /// <summary>
        /// 配置区域
        /// </summary>
        private void ConfigureArea(ControllerModel controller, UseApiAttribute useApiAttribute)
        {
            if (!controller.RouteValues.ContainsKey("area"))
            {
                if (!string.IsNullOrEmpty(useApiAttribute.Area))
                {
                    controller.RouteValues["area"] = useApiAttribute.Area;
                }
            }
        }

        private void ConfigureApiExplorer(ControllerModel controller)
        {
            if (string.IsNullOrWhiteSpace(controller.ApiExplorer.GroupName))
            {
                controller.ApiExplorer.GroupName = controller.ControllerName;
            }

            controller.ApiExplorer.IsVisible ??= true;
        }

        private void AddAppServiceSelector(string areaName, ActionModel action, ApiAttribute attribute)
        {
            var httpMethod = attribute.HttpMethod;

            var appServiceSelectorModel = action.Selectors[0];

            appServiceSelectorModel.AttributeRouteModel = CreateActionRouteModel(areaName, attribute);

            if (!appServiceSelectorModel.ActionConstraints.Any())
            {
                appServiceSelectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod.ToString() }));
                switch (attribute.HttpMethod)
                {
                    case HttpMethod.GET:
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpGetAttribute());
                        break;
                    case HttpMethod.POST:
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpPostAttribute());
                        break;
                    case HttpMethod.PUT:
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpPutAttribute());
                        break;
                    case HttpMethod.DELETE:
                        appServiceSelectorModel.EndpointMetadata.Add(new HttpDeleteAttribute());
                        break;
                    default:
                        throw new Exception($"不支持的 httpMethod: {httpMethod}.");
                }
            }
        }
        private AttributeRouteModel CreateActionRouteModel(string areaName, ApiAttribute attribute)
        {
            var apiPreFix = _dynamicWebApiOptions.DefaultApiPrefix;
            var route     = $"{apiPreFix}/{areaName}/{attribute.RouteUrl}".Replace("//", "/");
            return new AttributeRouteModel(new RouteAttribute(route));
        }

        /// <summary>
        /// 获取Api特性
        /// </summary>
        private ApiAttribute GetApiAttribute(ActionModel action)
        {
            var apiAttType = typeof(ApiAttribute);
            var apiAttObj  = action.Attributes.FirstOrDefault(o => o.GetType() == apiAttType);
            if (apiAttObj == null) return null;
            return (ApiAttribute)apiAttObj;
        }

        private void ConfigureParameters(ActionModel action)
        {
            foreach (var para in action.Parameters)
            {
                if (para.BindingInfo != null)
                {
                    continue;
                }

                if (!TypeHelper.IsPrimitiveExtendedIncludingNullable(para.ParameterInfo.ParameterType))
                {
                    if (CanUseFormBodyBinding(action, para))
                    {
                        para.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                    }
                }
            }
        }

        private bool CanUseFormBodyBinding(ActionModel action, ParameterModel parameter)
        {
            if (_dynamicWebApiOptions.FormBodyBindingIgnoredTypes.Any(t => t.IsAssignableFrom(parameter.ParameterInfo.ParameterType)))
            {
                return false;
            }

            foreach (var selector in action.Selectors)
            {
                if (selector.ActionConstraints == null)
                {
                    continue;
                }

                foreach (var actionConstraint in selector.ActionConstraints)
                {

                    var httpMethodActionConstraint = actionConstraint as HttpMethodActionConstraint;
                    if (httpMethodActionConstraint == null)
                    {
                        continue;
                    }

                    var httpMethod = new[] { "GET", "DELETE", "TRACE", "HEAD" };
                    if (httpMethodActionConstraint.HttpMethods.All(hm => httpMethod.Contains(hm)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}