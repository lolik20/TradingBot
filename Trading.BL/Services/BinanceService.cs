using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trading.BL.Interfaces;

namespace Trading.BL.Services
{
    public class BinanceService : IBinanceService
    {
        private HttpClient _httpClient;
        public BinanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BinancePair>> GetTickers()
        {
            var binance = await _httpClient.GetAsync("https://api.binance.com/api/v3/ticker/bookTicker");
            var binanceString = await binance.Content.ReadAsStringAsync();
            var binanceResult = JsonConvert.DeserializeObject<IEnumerable<BinancePair>>(binanceString);
            return binanceResult.Where(x => x.bidPrice != 0 && x.askPrice != 0).ToList();
        }
    }
    public class BinancePair
    {
        public decimal bidPrice { get; set; }
        public decimal askPrice { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public decimal lastPrice { get { return (bidPrice + askPrice) / 2; } set { lastPrice = 0; } }
        public string symbol { get; set; }
    }
}
