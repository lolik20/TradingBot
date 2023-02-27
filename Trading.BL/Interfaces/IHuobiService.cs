using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Trading.BL.Services.HuobiService;

namespace Trading.BL.Interfaces
{
    public interface IHuobiService
    {
        Task<List<HuobiPair>> GetTickers();
    }
}
