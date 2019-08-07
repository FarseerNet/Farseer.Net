namespace FS.Job.Entity
{
    public class JobSetting
    {
        /// <summary>
        /// 初始化设置
        /// </summary>
        /// <param name="index">排序（越小越前）</param>
        /// <param name="name">Job名称</param>
        /// <param name="enable">是否启用（默认启用）</param>
        public JobSetting(int index, string name, bool enable = true)
        {
            Index = index;
            Name = name;
            Enable = enable;
        }

        /// <summary>
        /// Job名称
        /// </summary>
        public string Name { get;}
        
        /// <summary>
        /// 是否启用（默认启用）
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// 排序（越小越前）
        /// </summary>
        public int Index { get; }
    }
}