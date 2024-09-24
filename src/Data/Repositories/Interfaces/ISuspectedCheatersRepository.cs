using Cs2Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Data.Repositories.Interfaces
{
    internal interface ISuspectedCheatersRepository
    {
        Task<SuspectedCheater> CreateAsync(SuspectedCheater suspectedCheater);
        Task<List<SuspectedCheater>> GetAllAsync();
        Task<List<SuspectedCheater>> GetAllUnbannedCheaters();
        Task<SuspectedCheater?> GetByCheaterUserIdAsync(string cheaterUserId);
        Task<SuspectedCheater?> SetBannedAsync(string cheaterUserId);
        Task<SuspectedCheater?> SetUnbannedAsync(string cheaterUserId);
    }
}
