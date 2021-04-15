namespace FS.ElasticSearch
{
    /// <summary>
    ///     多张表带静态实例化的上下文
    /// </summary>
    /// <typeparam name="TPo"></typeparam>
    public class EsContext<TPo> : EsContext where TPo : EsContext<TPo>, new()
    {
        /// <summary>
        ///     通过配置，连接ElasticSearch
        /// </summary>
        /// <param name="configName">配置名称</param>
        protected EsContext(string configName) : base(configName) { }
        
        /// <summary>
        ///     静态实例
        /// </summary>
        public static TPo Data => Data<TPo>();
    }
}