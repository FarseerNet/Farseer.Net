using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FS.Fss
{
    /// <summary>
    ///     JOB配置
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = true)]
    public class FssJobAttribute : Attribute
    {
        /// <summary>
        ///     是否启用（默认为true）
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        ///     在FSS平台配置的JobTypeName
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        ///     是否启用调试（默认为false）
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        ///     开启调试状态后要启动的job
        /// </summary>
        public string DebugMetaData { get; set; }
    }
}