using Cs2Bot.Services.Interfaces;
using Discord.Interactions;
using InteractionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Modules
{
    public class SuspectedCheatersModule : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionHandler _handler;
        private ISteamService _steamService;
        // Add new Ban repository here

        public SuspectedCheatersModule(InteractionHandler handler, ISteamService steamService) 
        {
            _handler = handler;
            _steamService = steamService;
        }

        // /track [platform] [id]

    }
}
