using Cs2Bot.Models;

namespace Cs2Bot.Services.Interfaces
{
    public interface ISteamService
    {
        Task<List<SteamNewsPost>> GetLatestNewsPosts(int count, int steamAppId);
        Task<List<SteamUserBanData>> GetSteamUsersBanData(List<string> steamId64List);
        Task<CheaterProfile> GetSteamUserProfile(string playerId);
        Task<string> GetSteamId64FromVanityUrl(string vanityUrl);
    }
}