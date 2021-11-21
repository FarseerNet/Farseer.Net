using System;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Core.LinkTrack
{
    public abstract class BackgroundServiceTrace : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var track = FsLinkTrack.TrackBackgroundService(this.GetType().Name))
            {
                try
                {
                    await ExecuteJobAsync(stoppingToken);
                }
                catch (System.Exception e)
                {
                    track.Exception(e);
                    IocManager.Instance.Logger<BackgroundServiceTrace>().LogError(e, e.Message);
                }
            }
        }

        protected abstract Task ExecuteJobAsync(CancellationToken stoppingToken);
    }
}