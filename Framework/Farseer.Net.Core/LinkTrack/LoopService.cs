using System;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.AOP.LinkTrack;
using FS.DI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Core.LinkTrack
{
    public abstract class LoopService : BackgroundService
    {
        protected abstract TimeSpan SleepMs { get; set; }

        [TrackBackgroundService]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await ExecuteJobAsync(stoppingToken);
                }
                catch (System.Exception e)
                {
                    IocManager.Instance.Logger<BackgroundServiceTrace>().LogError(e, e.Message);
                }
                if (SleepMs.TotalMilliseconds > 0) await Task.Delay(SleepMs, stoppingToken);
            }
        }

        protected abstract Task ExecuteJobAsync(CancellationToken stoppingToken);
    }
}