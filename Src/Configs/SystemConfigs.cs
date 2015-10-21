using System;

namespace FS.Configs
{
    /// <summary> 系统配置文件 </summary>
    public class SystemConfigs : AbsConfigs<SystemConfig>
    {
    }

    /// <summary> 系统配置文件 </summary>
    [Serializable]
    public class SystemConfig
    {
        /// <summary> 根据自己的需要来设置 </summary>
        public bool DeBug = false;

        /// <summary> 开启记录数据库执行过程 </summary>
        public bool IsWriteDbLog = false;

        /// <summary> 开启记录数据库错误记录 </summary>
        public bool IsWriteDbExceptionLog = false;

        /// <summary> 开启运行时错误记录发送邮件 </summary>
        public bool IsWriteRunExceptionLog = false;

        /// <summary> 开启数据库错误记录发送邮件 </summary>
        public bool IsSendExceptionEMail = false;
    }
}