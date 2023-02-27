using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common.ResponseModels;

namespace Trading.BL.Interfaces
{
    public interface ITelegramService
    {
        Task SendMessage(SymbolPrice message);
    }
}
