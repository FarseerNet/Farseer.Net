using FS.Data;

namespace Farseer.Net.DataDemo
{
    /// <summary>
    /// 元信息上下文
    /// </summary>
    public class MysqlContext : DbContext<MysqlContext>
    {
        public MysqlContext() : base("default")
        {
        }

        public TableSet<TaskGroupPO> TaskGroup { get; set; }

        protected override void CreateModelInit()
        {
            TaskGroup.SetName("task_group");
        }
    }
}