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
    public enum eumLogType
    {
        [Display(Name= "错误信息")]
        Error,
        [Display(Name = "调试信息")]
        Debug,
        [Display(Name = "严重信息")]
        Fatal,
        [Display(Name = "一般信息")]
        Info,
        [Display(Name = "警告信息")]
        Warn
    }
}
