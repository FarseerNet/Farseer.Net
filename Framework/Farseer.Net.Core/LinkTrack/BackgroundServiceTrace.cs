using System.Threading;
using System.Threading.Tasks;
using FS.Core.AOP.LinkTrack;
using FS.DI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Core.LinkTrack
{
    public abstract class BackgroundServiceTrace : BackgroundService
    {
        [TrackBackgroundService]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await ExecuteJobAsync(stoppingToken);
            }
            catch (System.Exception e)
            {
                IocManager.Instance.Logger<BackgroundServiceTrace>().LogError(e, e.Message);
            }
        }

        protected abstract Task ExecuteJobAsync(CancellationToken stoppingToken);
    }
}