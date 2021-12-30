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
        public Task<List<TaskGroupPO>> ToListAsync() => MysqlContext.Data.TaskGroup.ToListAsync();

        /// <summary>
        /// 获取任务组信息
        /// </summary>
        public Task<TaskGroupPO> ToEntityAsync(int id) => MysqlContext.Data.TaskGroup.Where(o => o.Id == id).ToEntityAsync();

        /// <summary>
        /// 更新任务组信息
        /// </summary>
        public Task UpdateAsync(int id, TaskGroupPO taskGroup) => MysqlContext.Data.TaskGroup.Where(o => o.Id == id).UpdateAsync(taskGroup);

        /// <summary>
        /// 添加任务组
        /// </summary>
        public async Task<int> AddAsync(TaskGroupPO po)
        {
            await MysqlContext.Data.TaskGroup.InsertAsync(po, true);
            return po.Id.GetValueOrDefault();
        }

        /// <summary>
        /// 删除当前任务组下的所有
        /// </summary>
        public async Task DeleteAsync(int taskGroupId)
        {
            await MysqlContext.Data.TaskGroup.Where(o => o.Id == taskGroupId).DeleteAsync();
        }
    }
}