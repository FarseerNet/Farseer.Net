using System.Threading;
using System.Threading.Tasks;
using FS.Job.Entity;

namespace FS.Job
{
    /// <summary>
    /// 需要接入Job执行的，必须承继该接口
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Job名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 计划设置
        /// </summary>
        JobSetting Setting { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 开始执行
        /// </summary>
        void Start(CancellationToken token);
        /// <summary>
        /// 结束
        /// </summary>
        void Stop();
    }
}