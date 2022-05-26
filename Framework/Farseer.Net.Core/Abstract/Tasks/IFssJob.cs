using System.Threading.Tasks;
using FS.Core.Abstract.Fss;

namespace FS.Core.Abstract.Tasks;

public interface IJob
{
    /// <summary>
    ///     执行具体任务
    /// </summary>
    /// <param name="context"> 调用上下文 </param>
    Task Execute(ITaskContext context);
}