using FS.DI;
using Microsoft.Extensions.Logging;

namespace FS.Job
{
    /// <summary>
    /// 注册到服务端
    /// </summary>
    public class ServiceRegister
    {
        public void Register(string server)
        {
            IocManager.Instance.Logger<JobModule>().LogInformation($"注册到服务端,{server}");
        }
    }
}