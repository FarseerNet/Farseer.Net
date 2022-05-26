using System.Collections.Generic;

namespace FS.Core.Abstract.MQ.Queue
{
    public interface IQueueProduct
    {
        /// <summary>
        ///     发送数据
        /// </summary>
        /// <param name="data"> 数据 </param>
        void Send(object data);
        /// <summary>
        ///     发送数据
        /// </summary>
        /// <param name="datalist"> 数据 </param>
        void Send(List<object> datalist);
    }
}