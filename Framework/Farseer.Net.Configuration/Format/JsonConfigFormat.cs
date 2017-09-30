// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 11:18
// ********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Farseer.Net.Configuration.Format
{
    /// <summary>
    /// 配置文件的格式
    /// </summary>
    public class JsonConfigFormat : IConfigFormat
    {
        /// <inherit/>
        public string ExtensionName { get; set; } = ".json";

        /// <inherit/>
        public Dictionary<string, object> Resolver(Dictionary<Type, object> configBindList, string configContent)
        {
            try
            {
                // 先解析第一层
                var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(configContent);
                // 解析第二层（指定了强类型）
                foreach (var source in root.Keys.ToArray())
                {
                    root[source] = JsonConvert.DeserializeObject(root[source].ToString(), configBindList.FirstOrDefault(o => o.Key.Name == source).Key);
                }
                return root;
            }
            catch (Exception exp)
            {
                throw new Exception($"配置文件读取错误：{exp.Message}");
            }
        }

        /// <inherit/>
        public string Serialize(Dictionary<Type, object> configBindList)
        {
            var dic = new Dictionary<string, object>();
            foreach (var tuple in configBindList) { dic[tuple.Key.Name] = tuple.Value; }

            return JsonConvert.SerializeObject(dic, Formatting.Indented);
        }
    }
}