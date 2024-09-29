using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Guild?> GetByIdAsync(ulong guildId)
        {
            var guild = await _context.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (guild == null)
            {
                return null;
            }
            return guild;
        }

        public async Task<Guild?> DeleteAsync(ulong guildId)
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


        //Refactor this
        public async Task<Guild?> SetGuildAsActiveAsync(ulong guildId)
        {
            var guild = await _context.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (guild == null)
            {
                return null;
            }
            guild.IsActive = true;
            await _context.SaveChangesAsync();
            return guild;
        }

        public async Task<Guild?> SetGuildAsInactiveAsync(ulong guildId)
        {
            var guild = await _context.Guilds.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (guild == null)
            {
                return null;
            }
            guild.IsActive = false;
            await _context.SaveChangesAsync();
            return guild;
        }
    }
}
