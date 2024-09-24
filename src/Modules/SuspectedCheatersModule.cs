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
        [SlashCommand("track", "Track a suspected cheater and get notified if they are banned")]
        public async Task TrackSuspectedCheater([Summary(description: "The platform the suspect is using")] PlatformIDs platformID, [Summary(description: "The platform ID (e.g SteamID64, Faceit ID) of the suspect")] string cheaterUserId )
        {
            var selectedPlatform = platformID.ToString();
            if (selectedPlatform == "Steam") // Steam
            {
                await RespondAsync("Steam support is coming soon!");
            }
            if (selectedPlatform == "Faceit") // Steam
            {
                await RespondAsync("Faceit support is coming soon!");
            }
        }

    public enum PlatformIDs
        {
            Steam,
            Faceit
        }    
    
    
    }




}
