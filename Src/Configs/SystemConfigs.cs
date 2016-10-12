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

        /// <summary> 开启调试日志记录 </summary>
        public bool IsWriteDebugLog = true;

        /// <summary> 开启异常日志记录 </summary>
        public bool IsWriteErrorLog = true;

        /// <summary> 开启错误日志记录 </summary>
        public bool IsWriteFatalLog = true;

        /// <summary> 开启信息日志记录 </summary>
        public bool IsWriteInfoLog = true;

        /// <summary> 开启警告日志记录 </summary>
        public bool IsWriteWarnLog = true;

        /// <summary> 开启SQL运行日志记录 </summary>
        public bool IsWriteSqlRunLog = false;

        /// <summary> 开启SQL异常日志记录 </summary>
        public bool IsWriteSqlErrorLog = false;

        /// <summary> 开启异常错误记录发送邮件 </summary>
        public bool IsSendErrorEmail = false;
    }
}