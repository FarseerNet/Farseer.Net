// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-03 13:45
// ********************************************

using System;
using Newtonsoft.Json;
using Farseer.Net.Core.Net;
using Farseer.Net.Core.Net.Gateway;

namespace Farseer.Net.Core.MicroService
{
    /// <summary>
    /// 所有微服务的基类
    /// </summary>
    public class AbsBaseService
    {
        /// <summary>
        /// 微服务基类
        /// </summary>
        public AbsBaseService(GatewayHeaderVO header)
        {
            this.GatewayHeader = header;
        }

        /// <summary>
        ///     网关头部
        /// </summary>
        protected GatewayHeaderVO GatewayHeader { get; set; }

        /// <summary>
        /// 网关效验
        /// </summary>
        /// <returns></returns>
        protected ApiResponseJson GatewayValidate()
        {
            //1、內网校验,ip格式 192.168*,10.*,ipV6的地址：fe80::*,feco::*
            var ip = GatewayHeader.AppIP;
            if (!(ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip == "::1" || ip == "127.0.0.1" || ip.ToLower().StartsWith("fe80::") || ip.ToLower().StartsWith("feco::")))
            {
                return ApiResponseJson.Error($"网关Ip({ip})禁止访问");
            }

            #region 参数值校验
            if (GatewayHeader.GatewayTimestamp <= 0) { return ApiResponseJson.Error($"时间戳({GatewayHeader.GatewayTimestamp})错误"); }  // 时间戳有误
            if (string.IsNullOrEmpty(GatewayHeader.GatewayVer)) { return ApiResponseJson.Error("网关版本号不能为空"); }               // 网关版本号不能为空
            if (string.IsNullOrEmpty(GatewayHeader.AppID)) { return ApiResponseJson.Error("授权应用ID不能为空"); }                    // 授权应用ID
            if (string.IsNullOrEmpty(GatewayHeader.ServiceSign)) { return ApiResponseJson.Error("签名不能为空"); }                    // 签名为空
            #endregion

            //2、时间戳有效期校验
            var currentTimestamp = DateTime.Now.Subtract(Convert.ToDateTime("1970-1-1 08:00:00")).TotalSeconds;
            if (currentTimestamp - GatewayHeader.GatewayTimestamp > 60 * 10) { return ApiResponseJson.Error($"请求过期，时间戳({GatewayHeader.GatewayTimestamp})"); }

            if (GatewayHeader.BuilderSign() != GatewayHeader.ServiceSign) { return ApiResponseJson.Error($"签名校验失败，接收到的签名为({GatewayHeader.ServiceSign})"); }
            return ApiResponseJson.Success("网关效验通过");
        }

        /// <summary>
        /// 转换到对象（失败时，用defVal代替）
        /// </summary>
        /// <typeparam name="T">要转换的对象</typeparam>
        /// <param name="obj">json字符串</param>
        /// <param name="defVal">失败时，用defVal代替</param>
        protected T ToObject<T>(object obj, T defVal)
        {
            if (obj == null) { return defVal; }
            var json = obj.ToString();
            if (string.IsNullOrWhiteSpace(json)) { return defVal; }
            try { return JsonConvert.DeserializeObject<T>(json); }
            catch
            {
                return defVal;
            }
        }

        /// <summary>
        /// 转换到对象（失败时，用defVal代替）
        /// </summary>
        /// <typeparam name="T">要转换的对象</typeparam>
        /// <param name="obj">json字符串</param>
        protected T ToObject<T>(object obj) => ToObject(obj, default(T));
    }
}