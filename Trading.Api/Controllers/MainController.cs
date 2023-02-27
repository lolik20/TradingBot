using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text.Json.Nodes;
using Trading.BL.Interfaces;
using Trading.Common.ResponseModels;

namespace Trading.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        private readonly IBinanceService _binanceService;
        private readonly IByBitService _byBitService;
        private readonly IGateService _gateService;
        public MainController(IBinanceService binanceService, IByBitService byBitService, IGateService gateService)
        {
            _binanceService = binanceService;
            _byBitService = byBitService;
            _gateService = gateService;
        }
        [HttpGet]
        public async Task<IActionResult> Connect()
        {

            var binanceTask = _binanceService.GetTickers();
            var bybitTask = _byBitService.GetTickers();
            var gateTask = _gateService.GetTickers();

            await Task.WhenAll(binanceTask, bybitTask, gateTask);

            var binanceResult = binanceTask.Result;
            var bybitResult = bybitTask.Result;
            var gateResult = gateTask.Result;

            var symbols = binanceResult.Select(x => x.symbol)
.Intersect(bybitResult.result.list.Select(x => x.symbol))
.Intersect(gateResult.Select(x => x.currency_pair));

            var prices = symbols.Select(symbol =>
            {
                var binance = binanceResult.First(x => x.symbol == symbol);
                var bybit = bybitResult.result.list.First(x => x.symbol == symbol);
                var gate = gateResult.First(x => x.currency_pair == symbol);

                var buyExchange = "";
                var sellExchange = "";
                decimal minPrice = decimal.MaxValue;
                decimal maxPrice = decimal.MinValue;

                if (binance.askPrice < bybit.ask1Price && binance.askPrice < gate.lowest_ask)
                {
                    buyExchange = "Binance";
                    minPrice = binance.askPrice;
                }
                else if (bybit.ask1Price < binance.askPrice && bybit.ask1Price < gate.lowest_ask)
                {
                    buyExchange = "ByBit";
                    minPrice = bybit.ask1Price;
                }
                else
                {
                    buyExchange = "Gate";
                    minPrice = gate.lowest_ask;
                }

                if (binance.bidPrice > bybit.bid1Price && binance.bidPrice > gate.highest_bid)
                {
                    sellExchange = "Binance";
                    maxPrice = binance.bidPrice;
                }
                else if (bybit.bid1Price > binance.bidPrice && bybit.bid1Price > gate.highest_bid)
                {
                    sellExchange = "ByBit";
                    maxPrice = bybit.bid1Price;
                }
                else
                {
                    sellExchange = "Gate";
                    maxPrice = gate.highest_bid;
                }

                var spread = (maxPrice - minPrice) / minPrice * 100;

                return new SymbolPrice
                {
                    Symbol = symbol,
                    BinancePrice = binance.lastPrice,
                    BinanceAsk = binance.askPrice,
                    BinanceBid = binance.bidPrice,
                    ByBitPrice = bybit.lastPrice,
                    ByBitAsk = bybit.ask1Price,
                    ByBitBid = bybit.bid1Price,
                    GatePrice = gate.last,
                    GateAsk = gate.lowest_ask,
                    GateBid = gate.highest_bid,
                    BuyExchange = buyExchange,
                    SellExchange = sellExchange,
                    Spread = spread
                };
            });

            return Ok(prices.OrderByDescending(x => x.Spread));
        }

    }
   

}
