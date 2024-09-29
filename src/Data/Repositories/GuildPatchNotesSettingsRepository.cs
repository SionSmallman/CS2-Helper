using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cs2Bot.Data.Repositories
{
    public class GuildPatchNotesSettingsRepository : IGuildPatchNotesSettingsRepository
    {
        private readonly BotDbContext _context;

        public GuildPatchNotesSettingsRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<GuildPatchNotesSetting> CreateAsync(GuildPatchNotesSetting setting)
        {
            await _context.GuildPatchNotesSettings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public async Task<List<GuildPatchNotesSetting>> GetAllAsync()
        {
            return await _context.GuildPatchNotesSettings.ToListAsync();
        }

        public async Task<GuildPatchNotesSetting?> GetByGuildIdAsync(ulong guildId)
        {
            var settings = await _context.GuildPatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            return settings;
        }

        public async Task<GuildPatchNotesSetting?> UpdateChannelAsync(ulong guildId, ulong newChannelId)
        {
            var settings = await _context.GuildPatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            settings.ChannelId = newChannelId;
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<GuildPatchNotesSetting?> SetActiveAsync(ulong guildId)
        {
            var settings = await _context.GuildPatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
            if (settings == null)
            {
                return null;
            }
            settings.PatchNotesEnabled = true;
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<GuildPatchNotesSetting?> SetInactiveAsync(ulong guildId)
        {
            var settings = await _context.GuildPatchNotesSettings.SingleOrDefaultAsync(x => x.GuildId == guildId);
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
