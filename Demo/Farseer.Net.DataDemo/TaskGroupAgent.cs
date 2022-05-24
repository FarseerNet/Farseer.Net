using System.Collections.Generic;
using System.Threading.Tasks;
using FS.DI;

namespace Farseer.Net.DataDemo
{
    /// <summary>
    /// 任务组数据库层
    /// </summary>
    public class TaskGroupAgent : ISingletonDependency
    {
        /// <summary>
        /// 获取所有任务组列表
        /// </summary>
        public Task<List<UserPO>> ToListAsync() => MysqlContext.Data.User.ToListAsync();

        /// <summary>
        /// 获取任务组信息
        /// </summary>
        public Task<UserPO> ToEntityAsync(int id) => MysqlContext.Data.User.Where(o => o.Id == id).ToEntityAsync();

        /// <summary>
        /// 更新任务组信息
        /// </summary>
        public Task UpdateAsync(int id, UserPO user) => MysqlContext.Data.User.Where(o => o.Id == id).UpdateAsync(user);

        /// <summary>
        /// 添加任务组
        /// </summary>
        public async Task<int> AddAsync(UserPO po)
        {
            await MysqlContext.Data.User.InsertAsync(po, true);
            return po.Id.GetValueOrDefault();
        }

        /// <summary>
        /// 删除当前任务组下的所有
        /// </summary>
        public async Task DeleteAsync(int taskGroupId)
        {
            await MysqlContext.Data.User.Where(o => o.Id == taskGroupId).DeleteAsync();
        }
    }
}