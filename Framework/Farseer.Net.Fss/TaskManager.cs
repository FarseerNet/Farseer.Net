using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FS.Core.Http;
using FS.Core.Job;
using FS.Core.Net;
using FS.DI;
using FS.Fss.Configuration;
using FS.Fss.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FS.Fss
{
    /// <summary>
    ///     任务管理
    /// </summary>
    public class TaskManager
    {
        private static readonly Dictionary<string, string> _header = new() { { "ClientIp", JobModule.Client.ClientIp }, { "ClientId", JobModule.Client.Id.ToString() }, { "ClientName", JobModule.Client.ClientName }, { "ClientJobs", string.Join(separator: ",", value: JobModule.Client.Jobs) } };

        /// <summary>
        ///     到FSS平台拉取任务
        /// </summary>
        public static async Task<List<TaskVO>> PullAsync(int pullCount)
        {
            // 当前客户端支持的job
            var jobItemConfig = JobConfigRoot.Get();

            // 服务端地址
            foreach (var server in jobItemConfig.Server.ToLower().Split(','))
            {
                // 拉取任务的地址
                var url = server.StartsWith(value: "http") ? $"{server}/task/pull" : $"http://{server}/task/pull";
                try
                {
                    var api = await HttpPostJson.PostAsync<ApiResponseJson<List<TaskVO>>>(url: url, postData: new Dictionary<string, string> { { "TaskCount", pullCount.ToString() } }, headerData: _header);
                    if (!api.Status) continue;
                    
                    IocManager.Instance.Logger<TaskQueueList>().LogDebug(message: $"本次拉取{api.Data.Count}条任务");
                    return api.Data;
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<TaskManager>().LogError(message: e.Message);
                }
            }
            return new List<TaskVO>();
        }

        /// <summary>
        ///     上传任务状态
        /// </summary>
        /// <param name="request"> </param>
        public static async Task JobInvokeAsync(JobInvokeRequest request)
        {
            // 当前客户端支持的job
            var jobItemConfig = JobConfigRoot.Get();

            // 服务端地址
            foreach (var server in jobItemConfig.Server.ToLower().Split(','))
            {
                // 拉取任务的地址
                var url = server.StartsWith(value: "http") ? $"{server}/task/JobInvoke" : $"http://{server}/task/JobInvoke";
                try
                {
                    var api = await HttpPost.PostAsync<ApiResponseJson>(url: url, postData: JsonConvert.SerializeObject(value: request), headerData: _header, contentType: "application/json");
                    if (!api.Status)
                    {
                        throw new FssException(api.StatusMessage, statusCode: api.StatusCode);
                    }
                    return;
                }
                catch (FssException)
                {
                    throw;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            throw new Exception($"上传任务状态失败了，请检查服务端是否正常。");
        }
    }
}