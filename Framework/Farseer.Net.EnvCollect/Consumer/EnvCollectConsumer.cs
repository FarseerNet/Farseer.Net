using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.EC.Dal;
using FS.Extends;
using FS.MQ.Queue;
using FS.MQ.Queue.Attr;

namespace FS.EC.Consumer
{
    /// <summary>
    ///     消费客户端
    /// </summary>
    [Consumer(Enable = true, Name = "EnvCollect", PullCount = 1000, SleepTime = 500)]
    public class EnvCollectConsumer : IListenerMessage
    {
        public async Task<bool> Consumer(List<object> queueList)
        {
            var lst = queueList.Select(o => (LinkTrackContext)o).ToList();
            // 设置C#的调用链
            foreach (var linkTrackContext in lst) linkTrackContext.List.ForEach(action: o => o.SetCallStackTrace());

            await EnvCollectEsContext.Data.Host.InsertAsync(lst.Select(o => o.Map<HostPO>()).ToList());
            return true;
        }
        
        public Task<bool> FailureHandling(List<object> messages) => throw new NotImplementedException();
    }
}