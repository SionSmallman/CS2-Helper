using Cs2Bot.Models;

namespace Cs2Bot.Data.Repositories.Interfaces
{
    public interface IGuildRepository
    {
        Task<Guild> CreateAsync(Guild guild);
        Task<Guild?> DeleteAsync(long guildId);
        Task<List<Guild>> GetAllAsync();
        Task<Guild?> GetByIdAsync(long guildId);
    }
}