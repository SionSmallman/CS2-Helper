using Cs2Bot.Models.Entities;

namespace Cs2Bot.Services.Interfaces
{
    public interface ISuspectedCheaterService
    {
        public Task<List<SuspectedCheater>> CheckForNewBansAsync();
        public Task SendBanNotification(List<SuspectedCheater> newlyBannedCheaters);
    }
}
