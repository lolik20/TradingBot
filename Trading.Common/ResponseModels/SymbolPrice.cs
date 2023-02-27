using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Common.ResponseModels
{
    public class SymbolPrice
    {
        public string Symbol { get; set; }
        public decimal BinancePrice { get; set; }
        public decimal BinanceAsk { get; set; }
        public decimal BinanceBid { get; set; }
        public decimal ByBitPrice { get; set; }
        public decimal ByBitAsk { get; set; }
        public decimal ByBitBid { get; set; }
        public decimal GatePrice { get; set; }
        public decimal GateAsk { get; set; }
        public decimal GateBid { get; set; }
        public string BuyExchange { get; set; }
        public string SellExchange { get; set; }
        public decimal Spread { get; set; }

    }
}
