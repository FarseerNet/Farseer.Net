// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-23 22:50
// ********************************************

using FS.Configuration;
using FS.MQ.RocketMQ.Configuration;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.RocketMQ
{
    /// <summary>
    ///     RocketMQ管理器
    /// </summary>
    public class RocketMQManager : IRocketMQManager
    {
        private static readonly object ObjLock = new object();

        /// <summary>
        ///     创建消息队列属性
        /// </summary>
        private ONSFactoryProperty _factoryInfo;

        public ONSFactoryProperty FactoryInfo => _factoryInfo ?? (_factoryInfo = InitFactoryProperty());

        /// <summary>
        ///     生产消息
        /// </summary>
        private IRocketMQOrderConsumer _orderConsumer;

        /// <summary>
        ///     生产消息
        /// </summary>
        private IRocketMQOrderProduct _orderProduct;

        /// <summary>
        ///     生产消息
        /// </summary>
        private IRocketMQProduct _product;

        /// <summary>
        ///     生产消息(基于http)
        /// </summary>
        private IHttpRocketMQProduct _httpProduct;

        /// <summary>
        ///     消费消息
        /// </summary>
        private IRocketMQConsumer _consumer;

        /// <summary>
        ///     消费消息(基于http)
        /// </summary>
        private IHttpRocketMQConsumer _httpConsumer;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly RocketMQItemConfig _config;
        
        /// <summary> RocketMQ管理器 </summary>
        public RocketMQManager(RocketMQItemConfig config)
        {
            _config = config;
        }

        /// <summary>
        ///     生产普通消息
        /// </summary>
        public IRocketMQProduct Product
        {
            get
            {
                if (_product != null) return _product;
                lock (ObjLock)
                {
                    return _product ?? (_product = new RocketMQProduct(FactoryInfo));
                }
            }
        }

        /// <summary>
        ///     生产普通消息（基于HTTP）
        /// </summary>
        public IHttpRocketMQProduct HttpProduct
        {
            get
            {
                if (_httpProduct != null) return _httpProduct;
                lock (ObjLock)
                {
                    return _httpProduct ?? (_httpProduct = new HttpRocketMQProduct(_config));
                }
            }
        }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        public IRocketMQConsumer Consumer
        {
            get
            {
                if (_consumer != null) return _consumer;
                lock (ObjLock)
                {
                    return _consumer ?? (_consumer = new RocketMQConsumer(FactoryInfo));
                }
            }
        }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        public IHttpRocketMQConsumer HttpConsumer
        {
            get
            {
                if (_httpConsumer != null) return _httpConsumer;
                lock (ObjLock)
                {
                    return _httpConsumer ?? (_httpConsumer = new HttpRocketMQConsumer(_config));
                }
            }
        }

        /// <summary>
        ///     生产顺序消息
        /// </summary>
        public IRocketMQOrderProduct OrderProduct
        {
            get
            {
                if (_orderProduct != null) return _orderProduct;
                lock (ObjLock)
                {
                    return _orderProduct ?? (_orderProduct = new RocketMQOrderProduct(FactoryInfo));
                }
            }
        }

        /// <summary>
        ///     消费顺序消息
        /// </summary>
        public IRocketMQOrderConsumer OrderConsumer
        {
            get
            {
                if (_orderConsumer != null) return _orderConsumer;
                lock (ObjLock)
                {
                    return _orderConsumer ?? (_orderConsumer = new RocketMQOrderConsumer(FactoryInfo));
                }
            }
        }

        private ONSFactoryProperty InitFactoryProperty()
        {
            var factoryInfo = new ONSFactoryProperty();
            if (_config.AccessKey != null) factoryInfo.setFactoryProperty(ONSFactoryProperty.AccessKey, _config.AccessKey);
            if (_config.SecretKey != null) factoryInfo.setFactoryProperty(ONSFactoryProperty.SecretKey, _config.SecretKey);
            if (_config.ConsumerID != null) factoryInfo.setFactoryProperty(ONSFactoryProperty.ConsumerId, _config.ConsumerID);
            if (_config.ProducerID != null) factoryInfo.setFactoryProperty(ONSFactoryProperty.ProducerId, _config.ProducerID);
            if (_config.Topic != null) factoryInfo.setFactoryProperty(ONSFactoryProperty.PublishTopics, _config.Topic);
            if (_config.Server != null) factoryInfo.setFactoryProperty(ONSFactoryProperty.NAMESRV_ADDR, _config.Server);

            // 集群订阅方式设置（不设置的情况下，默认为集群订阅方式）
            //factoryInfo.setFactoryProperty(ONSFactoryProperty.MessageModel, ONSFactoryProperty.CLUSTERING);

            // 广播订阅方式设置
            //factoryInfo.setFactoryProperty(ONSFactoryProperty.MessageModel, ONSFactoryProperty.BROADCASTING);

            // 设置线程数
            if (_config.ConsumeThreadNums < 1) _config.ConsumeThreadNums = 1;
            factoryInfo.setFactoryProperty(ONSFactoryProperty.ConsumeThreadNums, _config.ConsumeThreadNums.ToString());

            // 默认值为ONSChannel.ALIYUN，聚石塔用户必须设置为CLOUD，阿里云用户不需要设置(如果设置，必须设置为ALIYUN)
            factoryInfo.setOnsChannel(_config.Channel);

            if (_config.IsWriteLog) factoryInfo.setFactoryProperty(ONSFactoryProperty.LogPath, SysPath.LogPath);
            factoryInfo.setFactoryProperty(ONSFactoryProperty.SendMsgTimeoutMillis, "3000");
            return factoryInfo;
        }
    }
}