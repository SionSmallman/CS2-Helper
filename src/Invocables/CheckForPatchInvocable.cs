using Coravel.Invocable;
using Cs2Bot.Services.Interfaces;

namespace Cs2Bot.Invocables
{
    public class CheckForPatchInvocable : IInvocable
    {
        private readonly IPatchNotesService _patchNotesService;

        public CheckForPatchInvocable(IPatchNotesService patchNotesService)
        {
            _patchNotesService = patchNotesService;
        }

        public async Task Invoke()
        {
            var post = await _patchNotesService.CheckForNewPatchNotesAsync();
            if (post != null) 
            {
                await _patchNotesService.SendPatchNotesToSubscribedGuilds(post);
            }
        }
    }
}