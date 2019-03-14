using System;
using System.Threading;

namespace FS.Core.Queue.Core.SyncQueue
{
    /// <summary>
    /// 同步队列中的数据
    /// </summary>
    /// <typeparam name="T">原始数据</typeparam>
    /// <typeparam name="E">扩展数据</typeparam>
    public class SyncQueueData<T,E>
    {
        //构造时为0,Init()数据后,为3,入队-1,出队-1,处理完毕-1 达到0
        private readonly CountdownEvent _countdown = new CountdownEvent(0);
        private T _data = default(T);
        private E _extraData = default(E);
        public E ExtraData => this._extraData;

        public T Data => this._data;

        /// <summary>
        /// 是否完成了数据处理
        /// </summary>
        /// <returns></returns>
        public bool FinishedHandle()
        {
            return _countdown.IsSet;//计数是否为0
        }

        private void Init()
        {
            _countdown.Reset(3);//计数为3
        }

        /// <summary>
        /// 入队前,引用计数-1 = 2
        /// </summary>
        public void BeforeEnqueue()
        {
            _countdown.Signal();
        }
        /// <summary>
        /// 出队后,引用计数-1 = 1
        /// </summary>
        public void AfterDequeued()
        {
            _countdown.Signal();
        }
        /// <summary>
        /// 处理完毕后,引用计数-1 = 0
        /// </summary>
        public void AfterHandled()
        {
            _countdown.Reset(0);
        }

        public bool WaitHandled(TimeSpan timeSpan)
        {
            return _countdown.Wait(timeSpan);
        }
        public virtual void InitData(T data,E extraData)
        {
            this._data = data;
            this._extraData = extraData;
            this.Init();
        }

    }
}
