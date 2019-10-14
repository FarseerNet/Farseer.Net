using System;
using System.Net;
using FS.Configuration;
using FS.MQ.RocketMQ.Configuration;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.RocketMQ.Http
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
        private readonly ONSFactoryProperty _factoryInfo;

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
        ///     生产消息
        /// </summary>
        private IRocketMQConsumer _pushConsumer;

        /// <summary>
        /// 实例ID
        /// </summary>
        private readonly string _instanceID;
        
        /// <summary> RocketMQ管理器 </summary>
        public RocketMQManager(RocketMQItemConfig config)
        {
            
        }

        /// <summary>
        ///     生产消息
        /// </summary>
        public IRocketMQOrderProduct OrderProduct
        {
            get
            {
                throw new Exception("基于http的rocketmq未实现该方法");
                //if (_orderProduct != null) return _orderProduct;
                //lock (ObjLock)
                //{
                //    return _orderProduct ?? (_orderProduct = new RocketMQOrderProduct(_factoryInfo));
                //}
            }
        }

        /// <summary>
        ///     消费
        /// </summary>
        public IRocketMQOrderConsumer OrderConsumer
        {
            get
            {
                if (_orderConsumer != null) return _orderConsumer;
                lock (ObjLock)
                {
                    return _orderConsumer ?? (_orderConsumer = new RocketMQOrderConsumer(_factoryInfo));
                }
            }
        }

        /// <summary>
        ///     生产消息
        /// </summary>
        public IRocketMQProduct Product
        {
            get
            {
                if (_product != null) return _product;
                lock (ObjLock)
                {
                    return _product ?? (_product = new HttpRocketMQProduct(_factoryInfo,_instanceID));
                }
            }
        }

        /// <summary>
        ///     订阅消费
        /// </summary>
        public IRocketMQConsumer Consumer
        {
            get
            {
                if (_pushConsumer != null) return _pushConsumer;
                lock (ObjLock)
                {
                    return _pushConsumer ?? (_pushConsumer = new RocketMQPushConsumer(_factoryInfo));
                }
            }
        }
    }
}