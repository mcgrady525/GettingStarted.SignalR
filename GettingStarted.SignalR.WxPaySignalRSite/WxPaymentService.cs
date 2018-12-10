using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace GettingStarted.SignalR.WxPaySignalRSite
{
    public class WxPaymentService
    {
        private readonly static ConcurrentDictionary<string, PaymentResult> paymentResultDic = new ConcurrentDictionary<string, PaymentResult>();
        private readonly static Lazy<WxPaymentService> wxPaymentServiceInstance = new Lazy<WxPaymentService>(() => new WxPaymentService(GlobalHost.ConnectionManager.GetHubContext<WxPaymentHub>().Clients));
        private readonly Timer wxPaymentTimer;
        private readonly TimeSpan wxPaymentInterval = TimeSpan.FromMilliseconds(15000);
        private readonly object updateWxPaymentResultLock = new object();
        private volatile bool updateWxPaymentResult = false;
        
        private WxPaymentService(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            paymentResultDic.TryAdd("WxPaymentResult", new PaymentResult { IsSuccess = false });

            wxPaymentTimer = new Timer(NotifyWxPaymentResult, null, wxPaymentInterval, wxPaymentInterval);
        }

        /// <summary>
        /// 获取集线器的上下文
        /// </summary>
        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        /// <summary>
        /// 单例入口
        /// </summary>
        public static WxPaymentService Instance
        {
            get
            {
                return wxPaymentServiceInstance.Value;
            }
        }

        /// <summary>
        /// 通知支付结果
        /// </summary>
        /// <param name="state"></param>
        private void NotifyWxPaymentResult(object state)
        {
            var paymentResult= paymentResultDic["WxPaymentResult"];

            Clients.All.BroadWxPaymentResult(paymentResult);
        }

        /// <summary>
        /// 更新支付结果
        /// </summary>
        public void UpdateWxPaymentResult()
        {
            lock (updateWxPaymentResultLock)
            {
                if (!updateWxPaymentResult)
                {
                    updateWxPaymentResult = true;

                    foreach (var item in paymentResultDic.Values)
                    {
                        item.IsSuccess = true;
                    }

                    updateWxPaymentResult = false;
                }
            }

        }

    }

    /// <summary>
    /// 支付结果
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// 是否支付成功，false:未支付，true:已支付
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}