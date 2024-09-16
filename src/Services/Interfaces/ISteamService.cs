using Cs2Bot.Models;
using System.Text.Json.Nodes;

namespace Cs2Bot.Services.Interfaces
{
    public interface ISteamService
    {
        Task<List<SteamNewsPost>> GetLatestNewsPosts(int count, int steamAppId);
    }
}