using Coravel.Invocable;
using Cs2Bot.Services.Interfaces;

namespace Cs2Bot.Invocables
{
    public class CheckForNewBansInvocable : IInvocable
    {

        private readonly ISuspectedCheaterService _suspectedCheaterService;

        public CheckForNewBansInvocable(ISuspectedCheaterService suspectedCheaterService)
        {
            _suspectedCheaterService = suspectedCheaterService;
        }

        public async Task Invoke()
        {
            var bans = await _suspectedCheaterService.CheckForNewBansAsync();
            if (bans.Count > 0)
            {
                await _suspectedCheaterService.SendBanNotification(bans);
            }
        }
    }
}