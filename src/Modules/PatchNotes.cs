using Cs2Bot.Services.Interfaces;
using Discord;
using Discord.Interactions;
using InteractionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsBot_dotnet.src.Modules
{
    public class PatchNotes : InteractionModuleBase<SocketInteractionContext>
    {

        private InteractionHandler _handler;
        private ISteamService _steamService;

        public InteractionService Commands { get; set; }

        public PatchNotes(InteractionHandler handler, ISteamService steamService)
        {
            _handler = handler;
            _steamService = steamService;
        }

        [SlashCommand("hello", "Hello world!")]
        public async Task HelloAsync()
        {
            await RespondAsync("Hello world!");
        }

        [SlashCommand("hello2", "Hello world2!")]
        public async Task Hello2Async()
        {
            var newsPosts = await _steamService.GetLatestNewsPosts(3, 730);


            // check for patch notes
            // format contents
            // build embed
            // send embed
            await RespondAsync(newsPosts[0].ToString());
        }

        [SlashCommand("patchchannel", "Sets the channel where the bot will post patch notes")]
        public async Task SetPatchChannel([ChannelTypes(ChannelType.Text)] IChannel channel)
        {
            // edit db entry to set ReceivePatchNotes to true 
            await RespondAsync("Channel chosen");
        }
    }
}

// Plan for patch notes:
// - Hangfire task checks API every 2 minutes.
// - If new patch notes found, send to every discord that is active and sub'd to patch notes
// - Need to figure out how to track what channel they've subbed to. (patch notes table with chosen channel id?)