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
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Configuration;

namespace Trading.BL.Services
{
    public class TelegramService : ITelegramService
    {
        private TelegramBotClient _bot;
        private List<string> _users;
        private readonly IConfigurationSection _urls;
        public TelegramService(IConfiguration configuration)
        {
            _users = new List<string>() { "735342077", "662706906" };
            _bot = new TelegramBotClient("5460479060:AAFbjrFg4uVfZq4SMiSeua3HXP9Iah36P1A");
            _urls = configuration.GetSection("Urls");
        }

        public async Task SendMessage(SymbolPrice message)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                        new []
                        {
                            InlineKeyboardButton.WithUrl(message.BuyExchange,_urls[message.BuyExchange].Replace("$token",message.Symbol.Replace("USDT",""))),
                            InlineKeyboardButton.WithUrl(message.SellExchange,_urls[message.SellExchange].Replace("$token",message.Symbol.Replace("USDT",""))),
                        }
                    });
            foreach (var user in _users)
            {
                await _bot.SendTextMessageAsync(user, $"Пара: {message.Symbol}\nСпред: {Math.Round(message.Spread, 2)}%\nBinance: {message.BinancePrice}\nGate.io: {message.GatePrice}\nByBit: {message.ByBitPrice}\n{message.BuyExchange} -> {message.SellExchange}", replyMarkup: inlineKeyboard);

            }
        }


    }
}

