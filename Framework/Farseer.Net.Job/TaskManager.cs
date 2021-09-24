using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FS.Core;
using FS.Core.Http;
using FS.Core.Net;
using FS.DI;
using FS.Job.Configuration;
using FS.Job.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FS.Job
{
    /// <summary>
    ///     任务管理
    /// </summary>
    public class TaskManager
    {
        private static readonly Dictionary<string, string> _header = new() { { "ClientIp", JobModule.Client.ClientIp }, { "ClientId", JobModule.Client.Id.ToString() }, { "ClientName", JobModule.Client.ClientName }, { "ClientJobs", string.Join(separator: ",", value: JobModule.Client.Jobs) } };

        /// <summary>
        ///     自动拉取任务
        /// </summary>
        public void AutoPull()
        {
            // 开启任务，自动拉取任务
            Task.Factory.StartNew(function: async () =>
            {
                while (true)
                {
                    await PullAsync();
                    Thread.Sleep(millisecondsTimeout: 500);
                }
            }, creationOptions: TaskCreationOptions.LongRunning);
        }

        /// <summary>
        ///     到FSS平台拉取任务
        /// </summary>
        public static async Task PullAsync()
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
                    var json = await Net.PostAsync(url: url, postData: new Dictionary<string, string> { { "TaskCount", Environment.ProcessorCount.ToString() } }, headerData: _header, contentType: "application/json");
                    var api  = Jsons.ToObject<ApiResponseJson<List<TaskVO>>>(obj: json);
                    if (!api.Status) continue;
                    if (api.Data.Count == 0) return;

                    // 将任务添加到队列中
                    TaskQueueList.Enqueue(lstTask: api.Data);
                    return;
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<TaskManager>().LogError(exception: e, message: e.Message);
                }
            }
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
                    var json = await Net.PostAsync(url: url, postData: JsonConvert.SerializeObject(value: request), headerData: _header, contentType: "application/json");
                    var api  = Jsons.ToObject<ApiResponseJson>(obj: json);
                    if (!api.Status) IocManager.Instance.Logger<TaskManager>().LogWarning(message: api.StatusMessage);

                    return;
                }
                catch (Exception e)
                {
                    IocManager.Instance.Logger<TaskManager>().LogError(exception: e, message: e.Message);
                }
            }
        }
    }
}