using Cs2Bot.Models;

namespace Cs2Bot.Data.Repositories.Interfaces
{
    public interface IGuildPatchNotesSettingsRepository
    {
        Task<GuildPatchNotesSetting> CreateAsync(GuildPatchNotesSetting setting);
        Task<List<GuildPatchNotesSetting>> GetAllAsync();
        Task<GuildPatchNotesSetting?> GetByGuildIdAsync(ulong guildId);
        Task<GuildPatchNotesSetting?> SetActiveAsync(ulong guildId);
        Task<GuildPatchNotesSetting?> SetInactiveAsync(ulong guildId);
        Task<GuildPatchNotesSetting?> UpdateChannelAsync(ulong guildId, ulong newChannelId);
    }
}