using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trading.BL.Interfaces;

namespace Trading.BL.Services
{
    public class GateService : IGateService
    {
        private readonly HttpClient _httpClient;
        public GateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<GatePair>> GetTickers()
        {
            var response = await _httpClient.GetAsync("https://api.gateio.ws/api/v4/spot/tickers");
            string responseString = await response.Content.ReadAsStringAsync();
            IEnumerable<GatePair> unsortedResult = JsonConvert.DeserializeObject<IEnumerable<GatePair>>(responseString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var result = unsortedResult.Where(x=>x.highest_bid!=0&&x.lowest_ask!=0).ToList();
            result.ForEach(x => x.currency_pair = x.currency_pair.Replace("_", ""));
            return result;

        }
    }
    public class GatePair
    {
        public decimal highest_bid { get; set; }
        public decimal lowest_ask { get; set; }
        public decimal last { get; set; }
        public string currency_pair { get; set; }
    }
}
