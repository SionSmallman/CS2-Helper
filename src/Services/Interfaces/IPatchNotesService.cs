using Cs2Bot.Models;

namespace Cs2Bot.Services.Interfaces
{
    public interface IPatchNotesService
    {
        Task<SteamNewsPost> CheckForNewPatchNotesAsync();
        Task SendPatchNotesToSubscribedGuilds(SteamNewsPost patchNotesPost);
    }
}