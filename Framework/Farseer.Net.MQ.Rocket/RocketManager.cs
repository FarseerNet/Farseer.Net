// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-23 22:50
// ********************************************

using FS.Configuration;
using FS.MQ.Rocket.Configuration;
using FS.MQ.Rocket.Remove;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.Rocket
{
    /// <summary>
    ///     RocketMQ管理器
    /// </summary>
    public class RocketManager : IRocketManager
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
        private IRocketOrderConsumer _orderConsumer;

        /// <summary>
        ///     生产消息
        /// </summary>
        private IRocketOrderProduct _orderProduct;

        /// <summary>
        ///     生产消息
        /// </summary>
        private IRocketProduct _product;

        /// <summary>
        ///     生产消息(基于http)
        /// </summary>
        private IHttpRocketProduct _httpProduct;

        /// <summary>
        ///     消费消息
        /// </summary>
        private IRocketConsumer _consumer;

        /// <summary>
        ///     消费消息(基于http)
        /// </summary>
        private IHttpRocketConsumer _httpConsumer;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly RocketItemConfig _config;
        
        /// <summary> RocketMQ管理器 </summary>
        public RocketManager(RocketItemConfig config)
        {
            _config = config;
        }

        /// <summary>
        ///     生产普通消息
        /// </summary>
        public IRocketProduct Product
        {
            get
            {
                if (_product != null) return _product;
                lock (ObjLock)
                {
                    return _product ?? (_product = new RocketProduct(FactoryInfo));
                }
            }
        }

        /// <summary>
        ///     生产普通消息（基于HTTP）
        /// </summary>
        public IHttpRocketProduct HttpProduct
        {
            get
            {
                if (_httpProduct != null) return _httpProduct;
                lock (ObjLock)
                {
                    return _httpProduct ?? (_httpProduct = new HttpRocketProduct(_config));
                }
            }
        }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        public IRocketConsumer Consumer
        {
            get
            {
                if (_consumer != null) return _consumer;
                lock (ObjLock)
                {
                    return _consumer ?? (_consumer = new RocketConsumer(FactoryInfo));
                }
            }
        }

        /// <summary>
        ///     消费普通消息
        /// </summary>
        public IHttpRocketConsumer HttpConsumer
        {
            get
            {
                if (_httpConsumer != null) return _httpConsumer;
                lock (ObjLock)
                {
                    return _httpConsumer ?? (_httpConsumer = new HttpRocketConsumer(_config));
                }
            }
        }

        /// <summary>
        ///     生产顺序消息
        /// </summary>
        public IRocketOrderProduct OrderProduct
        {
            get
            {
                if (_orderProduct != null) return _orderProduct;
                lock (ObjLock)
                {
                    return _orderProduct ?? (_orderProduct = new RocketOrderProduct(FactoryInfo));
                }
            }
        }

        /// <summary>
        ///     消费顺序消息
        /// </summary>
        public IRocketOrderConsumer OrderConsumer
        {
            get
            {
                if (_orderConsumer != null) return _orderConsumer;
                lock (ObjLock)
                {
                    return _orderConsumer ?? (_orderConsumer = new RocketOrderConsumer(FactoryInfo));
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