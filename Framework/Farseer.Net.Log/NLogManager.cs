//using ZTO.Platform.Log.Configuration;

//namespace ZTO.Platform.Log
//{
//    /// <summary>
//    /// ES管理类
//    /// </summary>
//    public class NLogManager : INLogManager
//    {
//        //private readonly NLogConfig _config;
//        private static NLogClient _nLogClient;

//        public NLogClient Client { get; }

//        /// <summary>
//        /// 构造函数
//        /// </summary>

//        public NLogManager()
//        {
//            Client = CreateNLogInstance();
//        }

//        /// <summary>
//        /// 创建ElasticClient实例
//        /// </summary>
//        private NLogClient CreateNLogInstance()
//        {
//            if (null == _nLogClient)
//            {
//                _nLogClient = new NLogClient();
//            }
//            return _nLogClient;
//        }
//    }
//}
