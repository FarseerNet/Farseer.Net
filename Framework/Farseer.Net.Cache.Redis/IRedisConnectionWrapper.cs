using System;
using System.Net;
using StackExchange.Redis;

namespace FS.Cache.Redis
{
    /// <summary>
    /// Redis连接包装器接口
    /// </summary>
    public interface IRedisConnectionWrapper : IDisposable
    {
        /// <summary>
        /// 数据库
        /// </summary>
        /// <param name="db">db编号</param>
        /// <returns>数据库</returns>
        IDatabase Database(int? db = null);

        /// <summary>
        /// 服务器
        /// </summary>
        /// <param name="endPoint">终结点</param>
        /// <returns>服务器</returns>
        IServer Server(EndPoint endPoint);

        /// <summary>
        /// 获取终结点
        /// </summary>
        /// <returns>终结点列表</returns>
        EndPoint[] GetEndpoints();

        /// <summary>
        /// 清空Db
        /// </summary>
        /// <param name="db">db编号</param>
        void FlushDb(int? db = null);
    }
}
