using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trading.BL.Interfaces;
using Trading.Controllers;
using Newtonsoft.Json;
using Trading.Common.ResponseModels;
using Microsoft.Extensions.Configuration;

namespace Trading.Workers
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IBinanceService _binanceService;
        private readonly IByBitService _byBitService;
        private readonly IGateService _gateService;
        private readonly ITelegramService _telegramService;
        private readonly IHuobiService _huobiService;
        private readonly IConfiguration _configuration;
        private readonly List<string> _filter;
        public NotificationWorker(IBinanceService binanceService, IByBitService byBitService, IGateService gateService, ITelegramService telegramService, IConfiguration configuration, IHuobiService huobiService)
        {
            _huobiService = huobiService;
            _binanceService = binanceService;
            _byBitService = byBitService;
            _gateService = gateService;
            _telegramService = telegramService;
            _configuration = configuration;
            _filter = configuration.GetValue<string>("Filter").Split(',').ToList();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var binanceTask = _binanceService.GetTickers();
                var bybitTask = _byBitService.GetTickers();
                var gateTask = _gateService.GetTickers();
                //var huobiTask = _huobiService.GetTickers();
                await Task.WhenAll(binanceTask, bybitTask, gateTask);

                var binanceResult = binanceTask.Result;
                var bybitResult = bybitTask.Result;
                var gateResult = gateTask.Result;
                //var huobiResult = huobiTask.Result;

                var symbols = binanceResult.Select(x => x.symbol)
    .Intersect(bybitResult.result.list.Select(x => x.symbol))
    .Intersect(gateResult.Select(x => x.currency_pair));
    //.Intersect(huobiResult.Select(x => x.symbol));

                var prices = symbols.Select(symbol =>
                {

                    var binance = binanceResult.First(x => x.symbol == symbol);
                    var bybit = bybitResult.result.list.First(x => x.symbol == symbol);
                    var gate = gateResult.First(x => x.currency_pair == symbol);
                    //var huobi = huobiResult.First(x=>x.symbol==symbol);
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
                prices = prices.Where(x => !_filter.Any(t => x.Symbol.Contains(t)));
                var bestSpreadPair = prices.OrderByDescending(p => p.Spread).FirstOrDefault();
                if (bestSpreadPair.Spread > 1m)
                {
                    Console.WriteLine("Нашли спред");
                    await _telegramService.SendMessage(bestSpreadPair);
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ContinueWith(x => { });
            }
        }
    }
}
