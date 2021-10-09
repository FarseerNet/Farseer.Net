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
        ///     类属性
        /// </summary>
        internal PropertyInfo ClassProperty { get; }

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
            IndexName       = indexName.ToLower();
            ShardsCount     = shardsCount;
            ReplicasCount   = replicasCount;
            RefreshInterval = refreshInterval;
            
            if (aliasNames is
            {
                Length: > 0
            })
                AliasNames = aliasNames.Select(selector: aliasName => aliasName.ToLower()).ToArray();
            return this;
        }
    }
}