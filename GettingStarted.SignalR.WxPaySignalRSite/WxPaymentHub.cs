using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GettingStarted.SignalR.WxPaySignalRSite
{
    [HubName("wxPaymentHubMini")]
    public class WxPaymentHub : Hub
    {
        private readonly WxPaymentService wxPaymentService;

        public WxPaymentHub() : this(WxPaymentService.Instance) { }

        public WxPaymentHub(WxPaymentService wxPaymentService)
        {
            this.wxPaymentService = wxPaymentService;
        }

        /// <summary>
        /// 更新支付结果
        /// </summary>
        public bool UpdateWxPaymentResult()
        {
            wxPaymentService.UpdateWxPaymentResult();
            return true;
        }

    }
}