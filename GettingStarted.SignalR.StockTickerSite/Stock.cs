using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GettingStarted.SignalR.StockTickerSite
{
    /// <summary>
    /// 股票实体
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string Symbol { get; set; }
        
        private decimal _price;
        
        /// <summary>
        /// 股票价格
        /// </summary>
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price == value)
                {
                    return;
                }

                _price = value;

                if (DayOpen == 0)
                {
                    DayOpen = _price;
                }
            }
        }

        /// <summary>
        /// 开盘价格
        /// </summary>
        public decimal DayOpen { get; private set; }

        /// <summary>
        /// 涨幅
        /// </summary>
        public decimal Change
        {
            get
            {
                return Price - DayOpen;
            }
        }

        /// <summary>
        /// 涨幅百分比
        /// </summary>
        public double PercentChange
        {
            get
            {
                return (double)Math.Round(Change / Price, 4);
            }
        }
    }
}