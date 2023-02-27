using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Args;
using Trading.BL.Interfaces;
using Trading.Common.ResponseModels;

namespace Trading.BL.Services
{
    public class TelegramService : ITelegramService
    {
        private TelegramBotClient _bot;
        public TelegramService()
        {
            _bot = new TelegramBotClient("5460479060:AAFbjrFg4uVfZq4SMiSeua3HXP9Iah36P1A");
        }

        public async Task SendMessage(SymbolPrice message)
        {
            await _bot.SendTextMessageAsync("662706906", $"Пара: {message.Symbol}\nСпред: {Math.Round(message.Spread, 2)}%\nBinance: {message.BinancePrice}\nGate.io: {message.GatePrice}\nByBit: {message.ByBitPrice}\n{message.BuyExchange} -> {message.SellExchange}");
        }


    }
}

