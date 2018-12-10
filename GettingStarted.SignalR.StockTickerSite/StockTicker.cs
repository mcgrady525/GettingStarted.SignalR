using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GettingStarted.SignalR.StockTickerSite
{
    /// <summary>
    /// 股票价格Service
    /// </summary>
    public class StockTicker
    {
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(() => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));
        
        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();
        private readonly object _updateStockPricesLock = new object();
        private readonly double _rangePercent = .002;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();
        private readonly Timer _timer;
        private volatile bool _updatingStockPrices = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clients"></param>
        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _stocks.Clear();
            var stocks = new List<Stock>
            {
                new Stock { Symbol = "MSFT", Price = 800.31m },
                new Stock { Symbol = "APPL", Price = 578.18m },
                new Stock { Symbol = "GOOG", Price = 570.30m }
            };
            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));

            _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);
        }

        /// <summary>
        /// 单例入口
        /// </summary>
        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
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
        /// 获取所有股票价格
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        /// <summary>
        /// 回调方法，更新股票价格
        /// </summary>
        /// <param name="state"></param>
        private void UpdateStockPrices(object state)
        {
            lock (_updateStockPricesLock)
            {
                //检查是否另一个线程正在更新股票价格
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var stock in _stocks.Values)
                    {
                        if (TryUpdateStockPrice(stock))
                        {
                            BroadcastStockPrice(stock);
                        }
                    }

                    _updatingStockPrices = false;
                }
            }
        }

        /// <summary>
        /// 随机更新股票价格
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        private bool TryUpdateStockPrice(Stock stock)
        {
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
            {
                return false;
            }
            
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > .51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        /// <summary>
        /// 通知所有连接的客户端
        /// </summary>
        /// <param name="stock"></param>
        private void BroadcastStockPrice(Stock stock)
        {
            Clients.All.updateStockPrice(stock);
        }

    }
}