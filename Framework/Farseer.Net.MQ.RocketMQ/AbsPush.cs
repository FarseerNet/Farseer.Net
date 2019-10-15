using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using FS.DI;
using FS.MQ.RocketMQ.SDK;

namespace FS.MQ.RocketMQ
{
    /// <summary>
    /// RocketMQ推送
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class AbsPush<TEntity>
    {
        /// <summary>
        ///     RocketMQ管理器
        /// </summary>
        private readonly IRocketMQManager _mqManager;

        /// <summary>
        ///     配置文件的Name节点
        /// </summary>
        protected string ConfigName { get; }

        /// <summary>
        ///     标签
        /// </summary>
        protected string TagName { get; }

        /// <summary>
        ///     失败重试次数
        /// </summary>
        protected int TryCount { get; }

        /// <summary>
        ///     失败重试等待时间
        /// </summary>
        protected int TryWaitMillisecond { get; }

        /// <summary>
        /// RocketMQ推送
        /// </summary>
        /// <param name="configName">配置文件的Name节点</param>
        /// <param name="tagName">标签</param>
        /// <param name="tryCount">失败重试次数</param>
        /// <param name="tryWaitMillisecond">失败重试等待时间</param>
        protected AbsPush(string configName, string tagName, int tryCount = 3, int tryWaitMillisecond = 100)
        {
            ConfigName = configName;
            TagName = tagName;
            TryCount = tryCount;
            TryWaitMillisecond = tryWaitMillisecond;
            _mqManager = IocManager.Instance.Resolve<IRocketMQManager>(ConfigName);
        }

        /// <summary>
        ///     错误消息
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        ///     将内容发布到MQ
        /// </summary>
        public void Push(List<TEntity> lst)
        {
            Check.NotNull(lst);

            foreach (var entity in lst) Push(entity);
        }

        /// <summary>
        ///     将内容发布到MQ
        /// </summary>
        public void Push(TEntity entity, string key = null)
        {
            if (entity == null) return;
            for (var i = 0; i < TryCount; i++)
            {
                if (TryPush(entity, key)) break;
                Thread.Sleep(TryWaitMillisecond);
            }
        }

        /// <summary>
        /// 尝试推送
        /// </summary>
        private bool TryPush(TEntity entity, string key)
        {
            // 消息
            var message = JsonConvert.SerializeObject(entity);
            ErrorMessage = null;
            try
            {
                var sendResultOns = _mqManager.Product.Send(message, TagName, key);
                PushSuccess(sendResultOns, message, TagName, key);
                return true;
            }
            catch (Exception exp)
            {
                ErrorMessage = exp.Message;
                //LogService.Error($"message={message == null}_{exp}", $"{tagName}推送失败");
                return PushException(entity, exp);
            }
        }

        /// <summary>
        /// 当推送失败时，需要做的异常处理
        /// </summary>
        protected virtual bool PushException(TEntity entity, Exception exp) => false;

        /// <summary>
        /// 当推送成功后，回调
        /// </summary>
        protected virtual void PushSuccess(SendResultONS sendResultOns, string message, string tag = null, string key = null) { }

        /// <summary>
        ///     关闭连接
        /// </summary>
        public void Close() => _mqManager.Product.Close();
    }
}