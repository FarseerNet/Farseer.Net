namespace FS.Core.Queue.Core.AsyncQueue
{
    /// <summary>
    /// 同步队列中的数据
    /// </summary>
    /// <typeparam name="T">原始数据</typeparam>
    /// <typeparam name="E">扩展数据</typeparam>
    public class AsyncQueueData<T,E>
    {
        private T _data = default(T);
        private E _extraData = default(E);
        public E ExtraData => this._extraData;

        public T Data => this._data;
        
        public virtual void InitData(T data,E extraData)
        {
            this._data = data;
            this._extraData = extraData;
        }

    }
}
