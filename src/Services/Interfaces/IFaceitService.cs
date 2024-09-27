using Cs2Bot.Models;

namespace Cs2Bot.Services.Interfaces
{
    public interface IFaceitService
    {
        Task<string> GetFaceitIdFromNickname(string faceitNickname);
        Task<List<FaceitBanData>> GetFaceitUsersBanData(List<string> playerIds);
        Task<CheaterProfile> GetFaceitUserProfile(string playerId); 
    }
}