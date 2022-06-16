using System.Threading.Tasks;
using FS.Core.Net;

namespace FS.Extend;

public static class ApiResponseJsonExtend
{
    public static async Task<ApiResponseJson> ToSuccessAsync(this Task task, string statusMessage = "执行成功")
    {
        await task;
        return await ApiResponseJson.SuccessAsync(statusMessage);
    }
    
    public static async Task<ApiResponseJson<TData>> ToSuccessAsync<TData>(this Task<TData> task, string statusMessage = "执行成功")
    {
        return await ApiResponseJson<TData>.SuccessAsync(statusMessage, await task);
    }

    public static ApiResponseJson<TData> ToSuccess<TData>(this TData task, string statusMessage = "执行成功")
    {
        return ApiResponseJson<TData>.Success(statusMessage, task);
    }
}