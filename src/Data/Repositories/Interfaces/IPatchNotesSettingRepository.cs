using Cs2Bot.Models;

namespace Cs2Bot.Data.Repositories.Interfaces
{
    public interface IPatchNotesSettingRepository
    {
        Task<PatchNotesSetting> CreateAsync(PatchNotesSetting setting);
        Task<List<PatchNotesSetting>> GetAllAsync();
        Task<PatchNotesSetting?> GetByGuildIdAsync(ulong guildId);
        Task<PatchNotesSetting?> SetActiveAsync(ulong guildId);
        Task<PatchNotesSetting?> SetInactiveAsync(ulong guildId);
        Task<PatchNotesSetting?> UpdateChannelAsync(ulong guildId, ulong newChannelId);
    }
}