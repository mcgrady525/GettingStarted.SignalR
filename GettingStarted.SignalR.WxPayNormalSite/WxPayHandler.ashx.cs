using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SSharing.Frameworks.Common.Extends;
using System.Threading;
using System.Collections.Concurrent;

namespace GettingStarted.SignalR.WxPayNormalSite
{
    /// <summary>
    /// WxPayHandler 的摘要说明
    /// </summary>
    public class WxPayHandler : IHttpHandler
    {
        /// <summary>
        /// 保存支付结果信息
        /// </summary>
        private static ConcurrentDictionary<string, PaymentResult> paymentResultDic = new ConcurrentDictionary<string, PaymentResult>();
        private readonly object updatePaymentResultLock = new object();
        private volatile bool updatePaymentResult = false;

        public WxPayHandler()
        {
            paymentResultDic.TryAdd("PaymentResult", new PaymentResult { IsSuccess = false });
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var success = false;
            var msg = string.Empty;
            var action = context.Request.Params["action"];
            switch (action)
            {
                case "GetPaymentResult":
                    success = GetPaymentResult();
                    context.Response.Write(new { success = success, msg = msg }.ToJson());
                    break;
                case "NotifyPaymentResult":
                    success = NotifyPaymentResult();
                    if (success)
                    {
                        msg = "通知成功";
                    }
                    context.Response.Write(new { success = success, msg = msg }.ToJson());
                    break;
                default:
                    msg = "请求参数错误!";
                    context.Response.Write(new { success = success, msg = msg }.ToJson());
                    break;
            }
        }

        /// <summary>
        /// 获取支付结果
        /// </summary>
        /// <returns></returns>
        private bool GetPaymentResult()
        {
            var paymentResult = new PaymentResult { IsSuccess = false };
            paymentResultDic.TryGetValue("PaymentResult", out paymentResult);
            return paymentResult.IsSuccess;
        }

        /// <summary>
        /// 通知支付结果
        /// </summary>
        /// <returns></returns>
        private bool NotifyPaymentResult()
        {
            lock (updatePaymentResultLock)
            {
                if (!updatePaymentResult)
                {
                    updatePaymentResult = true;

                    foreach (var item in paymentResultDic.Values)
                    {
                        item.IsSuccess = true;
                    }

                    updatePaymentResult = false;
                }
            }

            return true;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 支付结果
    /// </summary>
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
    }
}