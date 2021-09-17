using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using FS.Core.LinkTrack;
using FS.DI;
using FS.MQ.Rabbit.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FS.MQ.Rabbit
{
    /// <summary>
    /// 批量拉取消息
    /// </summary>
    public class RabbitConsumerBatch
    {
        /// <summary>
        /// ioc
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        /// 创建消息队列属性
        /// </summary>
        private readonly RabbitConnect _connect;

        /// <summary>
        /// 消费监听
        /// </summary>
        private readonly string _consumerTypeName;

        /// <summary>
        /// 创建连接会话对象
        /// </summary>
        private IModel _channel;

        /// <summary>
        /// 批量拉取消息数量
        /// </summary>
        private readonly uint _batchPullMessageCount;

        /// <summary>
        /// 队列名称
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        /// 消费客户端
        /// </summary>
        /// <param name="iocManager">IOC</param>
        /// <param name="rabbitItemConfig"></param>
        /// <param name="queueName">队列名称</param>
        /// <param name="batchPullMessageCount">批量拉取消息数量（默认10）</param>
        /// <param name="consumerType">消费监听 </param>
        public RabbitConsumerBatch(IIocManager iocManager, Type consumerType, RabbitItemConfig rabbitItemConfig, string queueName, uint batchPullMessageCount)
        {
            this._iocManager            = iocManager;
            this._connect               = new RabbitConnect(rabbitItemConfig);
            this._batchPullMessageCount = batchPullMessageCount < 1 ? 10 : batchPullMessageCount;
            this._queueName             = queueName;
            this._consumerTypeName      = consumerType.FullName;
            if (!iocManager.IsRegistered(consumerType.FullName)) iocManager.Register(consumerType, consumerType.FullName, DependencyLifeStyle.Transient);
        }

        /// <summary>
        /// 循环拉取
        /// </summary>
        /// <param name="waitSleep">拉到消息处理完后，休息多长时间，继续下一轮拉取</param>
        /// <param name="autoAck">是否自动确认消息</param>
        public async Task<int> StartWhile(int waitSleep, bool autoAck = false)
        {
            // 采用轮询的方式持续拉取
            while (true)
            {
                var result = 0;
                try
                {
                    // 拉取消息
                    result = await Start(autoAck);
                }
                catch (Exception e)
                {
                    _iocManager.Logger<RabbitConsumerBatch>().LogError(e, $"{_consumerTypeName}消费异常：{e.Message}");
                }

                await Task.Delay(result > 0 ? waitSleep : 500);
            }
        }

        /// <summary>
        /// 监控消费（只消费一次）
        /// </summary>
        /// <param name="autoAck">是否自动确认，默认false</param>
        public async Task<int> Start(bool autoAck = false)
        {
            Connect();
            var   lstResult         = new List<string>();
            var   lstBasicGetResult = new List<BasicGetResult>();
            ulong deliveryTag       = 0;

            var listener = _iocManager.Resolve<IListenerMessageBatch>(_consumerTypeName);

            var result = false;
            try
            {
                var queueDeclarePassive = _channel.QueueDeclarePassive(_queueName);
                if (queueDeclarePassive.MessageCount == 0) return 0;
                // 并发拉取多条消息
                var forCount = queueDeclarePassive.MessageCount > _batchPullMessageCount ? _batchPullMessageCount : queueDeclarePassive.MessageCount;
                Parallel.For(0, forCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, index =>
                {
                    // 拉取一条消息
                    var resp = _channel.BasicGet(_queueName, autoAck);
                    if (resp == null) return;

                    lstBasicGetResult.Add(resp);

                    // 将byte消息转成string类型
                    var message = Encoding.UTF8.GetString(resp.Body.ToArray());
                    lstResult.Add(message);
                });

                // 没有取到数据时，直接退出
                if (lstResult.Count == 0) return 0;

                deliveryTag = lstBasicGetResult.Max(o => o.DeliveryTag);

                // 消费
                using (FsLinkTrack.TrackMqConsumer(_connect.Connection.Endpoint.ToString(), _queueName, "RabbitConsumerBatch"))
                {
                    result = await listener.Consumer(lstResult, lstBasicGetResult);
                }

                // 写入链路追踪
                if (_iocManager.IsRegistered<ILinkTrackQueue>()) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<RabbitConsumer>().LogError(e, e.Message);
                result = false;

                // 消费失败后处理
                try
                {
                    using (FsLinkTrack.TrackMqConsumer(_connect.Connection.Endpoint.ToString(), _queueName, "RabbitConsumer"))
                    {
                        result = await listener.FailureHandling(lstResult, lstBasicGetResult);
                    }

                    // 写入链路追踪
                    if (_iocManager.IsRegistered<ILinkTrackQueue>()) _iocManager.Resolve<ILinkTrackQueue>().Enqueue();
                }
                catch (Exception exception)
                {
                    IocManager.Instance.Logger<RabbitConsumer>().LogError(exception, "失败处理出现异常：" + listener.GetType().FullName);
                    result = false;
                }
            }
            finally
            {
                if (!autoAck)
                {
                    if (result) _channel.BasicAck(deliveryTag, true);
                    else _channel.BasicNack(deliveryTag, true, true); // 批量拒绝，代替_channel.BasicReject
                }

                Close();
            }

            // 成功时才返回前端数量，失败时，可由前端来决定休眠时间
            return result ? lstResult.Count : 0;
        }

        /// <summary>
        /// 单次消费连接MQ
        /// </summary>
        private void Connect()
        {
            if (_connect.Connection == null || !_connect.Connection.IsOpen) _connect.Open();
            if (_channel == null || _channel.IsClosed) _channel = _connect.Connection.CreateModel();
        }

        /// <summary>
        ///     关闭
        /// </summary>
        public void Close()
        {
            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
                _channel = null;
            }
        }
    }
}