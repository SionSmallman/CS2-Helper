using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Data.Repositories
{
    internal class SuspectedCheatersRepository : ISuspectedCheatersRepository
    {
        private readonly BotDbContext _context;

        public SuspectedCheatersRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<SuspectedCheater> CreateAsync(SuspectedCheater suspectedCheater)
        {
            await _context.SuspectedCheaters.AddAsync(suspectedCheater);
            await _context.SaveChangesAsync();
            return suspectedCheater;
        }

        public async Task<List<SuspectedCheater>> GetAllAsync()
        {
            return await _context.SuspectedCheaters.ToListAsync();
        }

        public async Task<List<SuspectedCheater>> GetAllUnbannedCheaters()
        {
            return await _context.SuspectedCheaters.Where(x => x.IsBanned == false).ToListAsync();

        }

        public async Task<SuspectedCheater?> GetByCheaterUserIdAsync(string cheaterUserId)
        {
            var suspect = await _context.SuspectedCheaters.SingleOrDefaultAsync(x => x.CheaterUserId == cheaterUserId);
            if (suspect == null)
            {
                return null;
            }
            return suspect;
        }

        public async Task<SuspectedCheater?> SetBannedAsync(string cheaterUserId)
        {
            var cheater = await _context.SuspectedCheaters.SingleOrDefaultAsync(x => x.CheaterUserId == cheaterUserId);
            if (cheater == null)
            {
                return null;
            }
            cheater.IsBanned = true;
            await _context.SaveChangesAsync();
            return cheater;
        }

        public async Task<SuspectedCheater?> SetUnbannedAsync(string cheaterUserId)
        {
            var cheater = await _context.SuspectedCheaters.SingleOrDefaultAsync(x => x.CheaterUserId == cheaterUserId);
            if (cheater == null)
            {
                return null;
            }
            cheater.IsBanned = false;
            await _context.SaveChangesAsync();
            return cheater;
        }
    }
}
