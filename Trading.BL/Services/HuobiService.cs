using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.BL.Interfaces;

namespace Trading.BL.Services
{
    public class HuobiService : IHuobiService
    {
        private readonly HttpClient _httpClient;
        public HuobiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<HuobiPair>> GetTickers()
        {
            var response = await _httpClient.GetAsync("https://api.huobi.pro/market/tickers");
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HuobiResponse>(responseString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return result.data;
        }
        public class HuobiResponse
        {
            public List<HuobiPair> data { get; set; }
        }
        public class HuobiPair
        {
            public decimal ask { get; set; }
            public decimal bid { get; set; }
            public string symbol
            {
                get
                {
                    return symbol.ToUpper();
                }
                set { symbol = value; }
            }
            [Newtonsoft.Json.JsonIgnore]
            public decimal price
            {
                get
                {
                    return (ask + bid) / 2;
                }
                set { price = 0; }
            }
        }
    }
}