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
    public class PatchNotesSettingRepository : IPatchNotesSettingRepository
    {
        private readonly BotDbContext _context;

        public PatchNotesSettingRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<PatchNotesSetting> CreateAsync(PatchNotesSetting setting)
        {
            await _context.PatchNotesSettings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public async Task<List<PatchNotesSetting>> GetAllAsync()
        {
            return await _context.PatchNotesSettings.ToListAsync();
        }

        public async Task<PatchNotesSetting?> GetByGuildIdAsync(ulong guildId)
        {
            var settings = await _context.PatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            return settings;
        }

        public async Task<PatchNotesSetting?> UpdateChannelAsync(ulong guildId, ulong newChannelId)
        {
            var settings = await _context.PatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            settings.ChannelId = newChannelId;
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<PatchNotesSetting?> SetActiveAsync(ulong guildId)
        {
            var settings = await _context.PatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            settings.PatchNotesEnabled = true;
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<PatchNotesSetting?> SetInactiveAsync(ulong guildId)
        {
            var settings = await _context.PatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            settings.PatchNotesEnabled = false;
            await _context.SaveChangesAsync();
            return settings;
        }

    }
}
