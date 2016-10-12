using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FS.Log
{
    /// <summary>
    /// 日志信息等级
    /// </summary>
    public enum EumLogType
    {
        /// <summary> 错误信息  </summary>
        [Display(Name= "错误信息")]
        Error,
        /// <summary> 调试信息  </summary>
        [Display(Name = "调试信息")]
        Debug,
        /// <summary> 严重信息  </summary>
        [Display(Name = "严重信息")]
        Fatal,
        /// <summary> 一般信息  </summary>
        [Display(Name = "一般信息")]
        Info,
        /// <summary> 警告信息  </summary>
        [Display(Name = "警告信息")]
        Warn
    }
}
