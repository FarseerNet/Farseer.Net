using System.Threading.Tasks;
using FS.Job.Entity;

namespace FS.Job
{
    public interface IFssJob
    {
        /// <summary>
        /// 执行具体任务
        /// </summary>
        /// <param name="context">调用上下文，可实时同步进度、日志到服务端</param>
        Task<bool> Execute(ReceiveContext context);
    }
}