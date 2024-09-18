using Coravel.Invocable;
using Cs2Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await _patchNotesService.CheckForNewPatchNotesAsync();
        }
    }
}