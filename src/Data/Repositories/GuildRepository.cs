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
    public class GuildRepository : IGuildRepository
    {
        private readonly BotDbContext _context;

        public GuildRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<Guild> CreateAsync(Guild guild)
        {
            await _context.Guilds.AddAsync(guild);
            await _context.SaveChangesAsync();
            return guild;
        }

        public async Task<List<Guild>> GetAllAsync()
        {
            return await _context.Guilds.ToListAsync();
        }

        public async Task<Guild?> GetByIdAsync(long guildId)
        {
            var guild = await _context.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (guild == null)
            {
                return null;
            }
            return guild;
        }

        public async Task<Guild?> DeleteAsync(long guildId)
        {
            var guild = await _context.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (guild == null)
            {
                return null;
            }
            _context.Guilds.Remove(guild);
            await _context.SaveChangesAsync();
            return guild;
        }
    }
}
