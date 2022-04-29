using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FS.ElasticSearch.Map
{
    /// <summary>
    ///     实体类结构映射
    /// </summary>
    public class SetDataMap
    {
        /// <summary>
        ///     设置索引、别名、分片数量、副本数量
        /// </summary>
        internal SetDataMap(KeyValuePair<PropertyInfo, SetPhysicsMap> entityPhysicsMap)
        {
            ClassProperty = entityPhysicsMap.Key;
            PhysicsMap    = entityPhysicsMap.Value;
        }

        /// <summary>
        ///     物理结构
        /// </summary>
        public SetPhysicsMap PhysicsMap { get; }

        /// <summary>
        ///     副本数量（默认1）
        /// </summary>
        public int ReplicasCount { get; private set; }

        /// <summary>
        ///     分片数量（默认3）
        /// </summary>
        public int ShardsCount { get; private set; }

        /// <summary>
        ///     刷新间隔（默认1秒）
        /// </summary>
        public int RefreshInterval { get; private set; }

        public string[] AliasNames { get; private set; }

        /// <summary>
        ///     索引名称
        /// </summary>
        public string IndexName { get; private set; }

        /// <summary>
        /// 索引日期格式
        /// </summary>
        public string IndexFormat { get; private set; }

        /// <summary>
        ///     类属性
        /// </summary>
        internal PropertyInfo ClassProperty { get; }

        /// <summary>
        ///     设置索引、别名、分片数量、副本数量
        /// </summary>
        /// <param name="indexName"> 库名称 </param>
        /// <param name="aliasNames"> 别名，多个使用数组 </param>
        public SetDataMap SetName(string indexName, params string[] aliasNames)
        {
            IndexName = indexName.ToLower();

            if (aliasNames is
                {
                    Length: > 0
                })
                AliasNames = aliasNames.Select(selector: aliasName => aliasName.ToLower()).ToArray();
            return this;
        }

        /// <summary>
        ///     同时设置索引的名称、别名，并按日期格式化(IndexFormat)索引名称
        /// </summary>
        /// <param name="indexName"> 库名称 </param>
        public SetDataMap SetName(string indexName)
        {
            if (string.IsNullOrWhiteSpace(IndexFormat)) throw new FarseerConfigException($"索引：{indexName}，缺少IndexFormat配置");
            SetName($"{indexName}_{DateTime.Now.ToString(format: IndexFormat)}", indexName);
            return this;
        }

        /// <summary>
        ///     设置索引、别名、分片数量、副本数量
        /// </summary>
        /// <param name="shardsCount"> </param>
        /// <param name="replicasCount"> </param>
        /// <param name="refreshInterval">刷新间隔 </param>
        /// <param name="indexFormat">索引格式 </param>
        public SetDataMap SetName(int shardsCount, int replicasCount, int refreshInterval, string indexFormat = null)
        {
            ShardsCount     = shardsCount;
            ReplicasCount   = replicasCount;
            RefreshInterval = refreshInterval;
            if (indexFormat != null) IndexFormat = indexFormat;

            return this;
        }

        /// <summary>
        ///     设置索引、别名、分片数量、副本数量
        /// </summary>
        /// <param name="indexName"> 库名称 </param>
        /// <param name="shardsCount"> </param>
        /// <param name="replicasCount"> </param>
        /// <param name="refreshInterval">刷新间隔 </param>
        /// <param name="aliasNames"> 别名，多个使用数组 </param>
        public SetDataMap SetName(string indexName, int shardsCount = 3, int replicasCount = 1, int refreshInterval = 1, params string[] aliasNames)
        {
            SetName(indexName, aliasNames);
            SetName(shardsCount, replicasCount, refreshInterval);
            return this;
        }
    }
}