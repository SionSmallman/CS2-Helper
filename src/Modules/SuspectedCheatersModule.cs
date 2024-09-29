using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models.Entities;
using Cs2Bot.Services.Interfaces;
using Discord.Interactions;
using InteractionFramework;

namespace Cs2Bot.Modules
{
    public class SuspectedCheatersModule : InteractionModuleBase<SocketInteractionContext>
    {
        private InteractionHandler _handler;
        private ISteamService _steamService;
        private IFaceitService _faceitService;
        private ISuspectedCheatersRepository _suspectedCheatersRepository;

        public SuspectedCheatersModule(InteractionHandler handler, ISteamService steamService, IFaceitService faceitService, ISuspectedCheatersRepository suspectedCheatersRepository) 
        {
            _handler = handler;
            _steamService = steamService;
            _faceitService = faceitService;
            _suspectedCheatersRepository = suspectedCheatersRepository;
        }

        // /track [platform] [id]
        [SlashCommand("track", "Track a suspected cheater and get notified if they are banned")]
        public async Task TrackSuspectedCheater(
            [Summary(description: "The platform the suspect is using")] PlatformIDs platformID, 
            [Summary(description: "The link to the suspects profile (e.g Steam profile link/Faceit profile link")] string profileLink
            )
        {
            var profileUri = new Uri( profileLink );
            var selectedPlatform = platformID.ToString();

            // Get user ID from profile link
            string suspectUserId;
            if (selectedPlatform == "Faceit")
            {
                // Faceit profile Url is formatted: https://www.faceit.com/en/players/{nickname}
                var faceitNickname = profileUri.Segments.Last();
                suspectUserId = await _faceitService.GetFaceitIdFromNickname(faceitNickname);
            }
            else
            {
                // Steam profile url is formatted: https://steamcommunity.com/profiles/{id or vanity url}/
                var steamUrlId = profileUri.Segments.Last().TrimEnd('/');
                
                // If the Url has letters, then they've used a custom vanity url instead of the SteamID64.
                // So resolve to SteamID64
                if (steamUrlId.Any(char.IsLetter)) {
                    suspectUserId = await _steamService.GetSteamId64FromVanityUrl(steamUrlId);
                }
                else
                {
                    suspectUserId = steamUrlId;
                }
            }

            // If not a valid suspect ID, return
            if (suspectUserId == null)
            {
                await RespondAsync("Could not find profile. Please check the profile link is correct and try again", ephemeral: true);
                return;
            }

            // If cheater already tracked in guild, return
            var existingSuspect = await _suspectedCheatersRepository.CheckIfSuspectAlreadyTrackedAsync(suspectUserId, Context.Guild.Id );
            if (existingSuspect != null) 
            {
                await RespondAsync("Suspect is already being tracked in this guild!", ephemeral: true);
                return;
            } 

            SuspectedCheater newSuspect = new SuspectedCheater()
            {
                CheaterUserId = suspectUserId,
                Platform = selectedPlatform,
                IsBanned = false,
                GuildId = Context.Guild.Id,
                DiscordUserId = Context.User.Id,
                ChannelId = Context.Channel.Id,
            };
            await _suspectedCheatersRepository.CreateAsync(newSuspect);
            await RespondAsync($"Now tracking: {profileLink}");
        }
        
    public enum PlatformIDs
        {
            Steam,
            Faceit
        }    
    }
}
