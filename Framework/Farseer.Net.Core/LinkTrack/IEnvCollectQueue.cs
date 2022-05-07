namespace FS.Core.LinkTrack
{
    public interface IEnvCollectQueue
    {
        /// <summary> 入队,满了就丢掉新数据 </summary>
        void Enqueue();
    }
}