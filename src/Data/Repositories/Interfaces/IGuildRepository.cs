using Cs2Bot.Models;

namespace Cs2Bot.Data.Repositories.Interfaces
{
    public interface IGuildRepository
    {
        Task<Guild> CreateAsync(Guild guild);
        Task<Guild?> DeleteAsync(ulong guildId);
        Task<List<Guild>> GetAllAsync();
        Task<Guild?> GetByIdAsync(ulong guildId);
        Task<Guild?> SetGuildAsActiveAsync(ulong guildId);
        Task<Guild?> SetGuildAsInactiveAsync(ulong guildId);
    }
}