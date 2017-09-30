// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 12:42
// ********************************************

using System;
using System.Collections.Generic;

namespace Farseer.Net.Configuration.Format
{
    /// <summary>
    /// 配置文件的格式
    /// </summary>
    public interface IConfigFormat
    {
        /// <summary>
        /// 文件扩展名
        /// </summary>
         string ExtensionName { get; set; }

        /// <summary>
        /// 解析读取出来的配置内容
        /// </summary>
        /// <param name="configBindList"></param>
        /// <param name="configContent">配置内容</param>
        Dictionary<string, object> Resolver(Dictionary<Type, object> configBindList, string configContent);

        /// <summary>
        /// 将内容解析成String
        /// </summary>
        /// <param name="configBindList">配置文件绑定列表</param>
        string Serialize(Dictionary<Type, object> configBindList);
    }
}