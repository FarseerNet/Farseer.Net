namespace FS.Job
{
    /// <summary>
    /// 需要接入Job执行的，必须承继该接口
    /// </summary>
    public interface IJob
    {
        string Name { get; }
        void Init();
        void Start();
        void Stop();
    }
}