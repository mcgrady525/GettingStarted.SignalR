using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GettingStarted.SignalR.WxPaySignalRSite.Startup))]

namespace GettingStarted.SignalR.WxPaySignalRSite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
