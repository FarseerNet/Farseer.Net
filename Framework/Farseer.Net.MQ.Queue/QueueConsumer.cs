using System;
using System.Threading.Tasks;
using FS.Core.Abstract.MQ.Queue;
using FS.Core.AOP;
using FS.Core.LinkTrack;
using FS.DI;
using FS.MQ.Queue.Configuration;
using Microsoft.Extensions.Logging;

namespace FS.MQ.Queue
{
    public class QueueConsumer
    {
        /// <summary>
        ///     消费监听
        /// </summary>
        private readonly string _consumerTypeName;

        /// <summary>
        ///     ioc
        /// </summary>
        private readonly IIocManager _iocManager;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly QueueConfig _queueConfig;

        /// <summary>
        /// 队列数据
        /// </summary>
        private readonly IQueueList _queueList;

        /// <summary>
        ///     消费客户端
        /// </summary>
        /// <param name="iocManager"> IOC </param>
        /// <param name="queueConfig">配置信息</param>
        /// <param name="consumerType"> 消费监听 </param>
        public QueueConsumer(IIocManager iocManager, Type consumerType, QueueConfig queueConfig)
        {
            _iocManager       = iocManager;
            _queueConfig      = queueConfig;
            _consumerTypeName = consumerType.FullName;
            _queueList        = IocManager.GetService<IQueueList>($"queue_list_{queueConfig.Name}");
            if (!iocManager.IsRegistered(name: consumerType.FullName)) iocManager.Register(type: consumerType, name: consumerType.FullName, lifeStyle: DependencyLifeStyle.Transient);
        }

        /// <summary>
        ///     循环拉取
        /// </summary>
        /// <param name="autoAck"> 是否自动确认消息 </param>
        public async Task StartWhile(bool autoAck = false)
        {
            // 采用轮询的方式持续拉取
            while (true)
            {
                try
                {
                    // 拉取消息
                    await Start(autoAck: autoAck);
                }
                catch (Exception e)
                {
                    _iocManager.Logger<QueueConsumer>().LogError(exception: e, message: $"{_consumerTypeName}消费异常：{e.Message}");
                }

                await Task.Delay(_queueConfig.SleepTime);
            }
        }

        /// <summary>
        ///     监控消费（只消费一次）
        /// </summary>
        /// <param name="autoAck"> 是否自动确认，默认false </param>
        public async Task<int> Start(bool autoAck = false)
        {
            using var lst = _queueList.Pull();
            if (lst == null) return 0;

            var consumerService = _iocManager.Resolve<IListenerMessage>(name: _consumerTypeName);
            var result          = false;
            try
            {
                // 消费
                result = await consumerService.Consumer(lst);
            }
            catch (Exception e)
            {
                IocManager.Instance.Logger<QueueConsumer>().LogError(exception: e, message: e.Message);
                result = false;

                // 消费失败后处理
                try
                {
                    result = await consumerService.FailureHandling(lst);
                }
                catch (Exception exception)
                {
                    IocManager.Instance.Logger<QueueConsumer>().LogError(exception: exception, message: "失败处理出现异常：" + consumerService.GetType().FullName);
                    result = false;
                }
            }
            finally
            {
                IocManager.Instance.Release(consumerService);
                if (!result && !autoAck && lst.Count > 0)
                {
                    _iocManager.Resolve<IQueueProduct>(_queueConfig.Name).Send(lst);
                }
            }

            // 成功时才返回前端数量，失败时，可由前端来决定休眠时间
            return result ? lst.Count : 0;
        }
    }
}