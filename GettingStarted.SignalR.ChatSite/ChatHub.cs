using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace GettingStarted.SignalR.ChatSite
{
    /// <summary>
    /// 集线器
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// 供客户端调用的方法，发送消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string name, string message)
        {
            //调用所有客户端的broadcastMessage方法
            Clients.All.broadcastMessage(name, message);
        }
    }
}