using System.Linq;
using FS.Core.Net;
using FS.Extends;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FS
{
    /// <summary>
    /// 错误的消息，使用Json格式返回
    /// </summary>
    public class BadRequestException : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 必填项错误
            if (context.Result is BadRequestObjectResult badResult)
            {
                if (badResult.Value is ValidationProblemDetails val)
                {
                    var lstError = val.Errors.SelectMany(o => o.Value).ToList();
                    var error    = lstError.ToString(",");
                    badResult.Value = ApiResponseJson.Error(error, statusCode: badResult.StatusCode.GetValueOrDefault());
                    return;
                }
            }

            // hread类型错误
            if (context.Result is ObjectResult objResult)
            {
                if (objResult.StatusCode == 415)
                {
                    objResult.Value = ApiResponseJson.Error("只能使用application/json的content-type请求", 415);
                }
            }
        }
    }
}