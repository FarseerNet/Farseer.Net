using System.Threading.Tasks;
using FS.Core.AOP.LinkTrack;

namespace FS.Core.Abstract.Fss
{
    public interface IFssJob
    {
        /// <summary>
        ///     执行具体任务
        /// </summary>
        /// <param name="context"> 调用上下文，可实时同步进度、日志到服务端 </param>
        [TrackFss]
        Task<bool> Execute(IFssContext context);
    }
}