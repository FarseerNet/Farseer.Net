using FS.Data;

namespace Farseer.Net.DataDemo
{
    /// <summary>
    /// 元信息上下文
    /// </summary>
    public class MysqlContext : DbContext<MysqlContext>
    {
        public MysqlContext() : base("test")
        {
        }

        public TableSet<UserPO> User { get; set; }

        /// <summary>
        /// 这里可以设置表名
        /// </summary>
        protected override void CreateModelInit()
        {
            User.SetName("user");
        }
    }
}