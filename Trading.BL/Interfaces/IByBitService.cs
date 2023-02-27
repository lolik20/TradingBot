using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.BL.Services;

namespace Trading.BL.Interfaces
{
    public interface IByBitService
    {
        Task<ByBitResponse> GetTickers();
    }
}
