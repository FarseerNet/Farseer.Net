using System;
using System.Net;
using System.Reflection;
using Farseer.Net.AspNetCore.Attribute;
using FS.Core.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Farseer.Net.AspNetCore.Filters;

/// <summary>
/// 统一的Api返回格式
/// </summary>
public class FarseerApiResultFilters : IResultFilter // IActionFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var methodInfo = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo;

        // 优先使用ApiAttribute
        ApiResponseAttribute apiResponseAtt;
        var                  apiAtt = methodInfo.GetCustomAttribute<ApiAttribute>();
        if (apiAtt == null) apiResponseAtt = methodInfo.GetCustomAttribute<ApiResponseAttribute>() ?? new ApiResponseAttribute(200);
        else { apiResponseAtt = new ApiResponseAttribute(apiAtt.StatusCode, apiAtt.Message); }

        // 根据实际需求进行具体实现
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value == null)
            {
                context.Result = new ObjectResult(new ApiResponseJson() { StatusCode = 404, StatusMessage = "未找到资源", Status = false });
            }
            else
            {
                if (objectResult.Value is ApiResponseJson result)
                {
                    context.Result = new ObjectResult(result);
                    return;
                }
                context.Result = new ObjectResult(new ApiResponseJson { Status = true, StatusCode = apiResponseAtt.StatusCode, StatusMessage = apiResponseAtt.Message, Data = objectResult.Value });
            }
        }
        else if (context.Result is EmptyResult)
        {
            context.Result = new ObjectResult(new ApiResponseJson { Status = true, StatusCode = apiResponseAtt.StatusCode, StatusMessage = apiResponseAtt.Message });
        }
        else if (context.Result is ContentResult contentResult)
        {
            context.Result = new ObjectResult(new ApiResponseJson { Status = true, StatusCode = apiResponseAtt.StatusCode, StatusMessage = apiResponseAtt.Message, Data = contentResult.Content });
        }
        else if (context.Result is StatusCodeResult codeResult)
        {
            context.Result = new ObjectResult(new ApiResponseJson { StatusCode = codeResult.StatusCode, StatusMessage = apiResponseAtt.Message });
        }
        else if (context.Result is Exception)
        {
            var result = context.Result as Exception;
            context.Result = new ObjectResult(new ApiResponseJson { StatusCode = 500, StatusMessage = result.Message });
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {

    }
}