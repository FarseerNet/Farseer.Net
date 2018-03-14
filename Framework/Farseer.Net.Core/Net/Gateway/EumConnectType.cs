namespace FS.Core.Net.Gateway
{
    /// <summary>
    /// 网关到微服务的通讯方式
    /// </summary>
    public enum EumConnectType
    {
        /// <summary>
        /// Http
        /// </summary>
        Http,
        /// <summary>
        /// Rpc
        /// </summary>
        Rpc,
        /// <summary>
        /// 内置Wcf
        /// </summary>
        Wcf
        ///// <summary>
        ///// Grpc
        ///// </summary>
        //Grpc,
    }
}

//namespace FS.Core.Net.Gateway
//{
//    /// <summary>
//    /// 网关到微服务的通讯方式
//    /// </summary>
//    public enum EumConnectType
//    {
//        /// <summary>
//        /// Http
//        /// </summary>
//        Http,
//        /// <summary>
//        /// RPC
//        /// </summary>
//        Remote,
//        /// <summary>
//        /// 内置Wcf
//        /// </summary>
//        Wcf,
//    }
//}