using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.BL.Interfaces;

namespace Trading.BL.Services
{
    public class ByBitService:IByBitService
    {
        private readonly HttpClient _httpClient;
        public ByBitService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ByBitResponse> GetTickers()
        {
            var bybit = await _httpClient.GetAsync("https://api.bybit.com/v5/market/tickers?category=spot");
            var bybitString = await bybit.Content.ReadAsStringAsync();
            var byBitResponse = JsonConvert.DeserializeObject<ByBitResponse>(bybitString);
            return byBitResponse;
        }
    }
    public class ByBitResponse
    {
        public ByBitResult result { get; set; }
    }
    public class ByBitResult
    {
        public List<ByBitPair> list { get; set; }
    }
    public class ByBitPair
    {
        public string symbol { get; set; }
        public decimal lastPrice { get; set; }
        public decimal bid1Price { get; set; }
        public decimal ask1Price { get; set;}
    }
}
