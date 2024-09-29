using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Cs2Bot.Models.Entities;
using Cs2Bot.Services.Interfaces;
using Discord;
using Discord.WebSocket;

namespace Cs2Bot.Services
{
    public class SuspectedCheaterService : ISuspectedCheaterService
    {
        
        private readonly ISuspectedCheatersRepository _suspectedCheatersRepository;
        private readonly IFaceitService _faceitService;
        private readonly ISteamService _steamService;
        private readonly DiscordSocketClient _client;

        public SuspectedCheaterService(ISuspectedCheatersRepository suspectedCheatersRepository, IFaceitService faceitService, ISteamService steamService, DiscordSocketClient client)
        {
            _suspectedCheatersRepository = suspectedCheatersRepository;
            _faceitService = faceitService;
            _steamService = steamService;
            _client = client;
        }
        
        public async Task<List<SuspectedCheater>> CheckForNewBansAsync()
        {
            var usersToCheck = await _suspectedCheatersRepository.GetAllUnbannedCheaters();

            // Output list of newly banned users
            var newlyBanned = new List<SuspectedCheater>();

            // Filter list by each platform, then check for new bans and add to output list if ban had occured
            var faceitUsers = new List<SuspectedCheater>();
            var steamUsers = new List<SuspectedCheater>();
            foreach (var user in usersToCheck)
            {
                if(user.Platform == "Faceit")
                {
                    faceitUsers.Add(user);
                } 
                else
                {
                    steamUsers.Add(user);
                }
            }
            var steamBannedUsers = await _steamService.GetSteamUsersBanData(steamUsers.Select(x => x.CheaterUserId).ToList());
            var faceitBannedUsers = await _faceitService.GetFaceitUsersBanData(faceitUsers.Select(x => x.CheaterUserId).ToList());

            // For each new ban, refetch the SuspectedUser associated with the bans user ID and add it to output list
            foreach (SteamUserBanData user in steamBannedUsers)
            {
                newlyBanned.Add(steamUsers.First(x => x.CheaterUserId == user.SteamId));
                await _suspectedCheatersRepository.SetBannedAsync(user.SteamId);
            }
            foreach (FaceitBanData user in faceitBannedUsers)
            {
                newlyBanned.Add(faceitUsers.First(x => x.CheaterUserId == user.User_Id));
                await _suspectedCheatersRepository.SetBannedAsync(user.User_Id);
            }

            return newlyBanned;
        }

        public async Task SendBanNotification(List<SuspectedCheater> newlyBannedCheaters)
        {
            
            // For each new ban, get tracking guild and channel and post the good news!
            foreach (var cheater in newlyBannedCheaters)
            {
                var trackingChannel = cheater.ChannelId;
                var trackingUser = _client.GetUser(cheater.DiscordUserId) as IUser;
                var guildChannel = _client.GetChannel((ulong)cheater.ChannelId!) as IMessageChannel;
                if (guildChannel == null)
                {
                    Console.WriteLine("Cheater was banned but tracked channel not found!");
                    Console.WriteLine("Cheater User ID: " + cheater.CheaterUserId);
                    Console.WriteLine("Guild ID: " + cheater.GuildId);
                    continue;
                }

                // Grab the profile details for the embed
                CheaterProfile profile;
                if (cheater.Platform == "Faceit")
                {
                    profile = await _faceitService.GetFaceitUserProfile(cheater.CheaterUserId);
                }
                else 
                {
                    profile = await _steamService.GetSteamUserProfile(cheater.CheaterUserId);
                }

                var embed = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder().WithName("CS2 Helper - New Ban detected!"),
                    Color = Color.Red,
                    Title = $"User banned - {profile.Nickname}",
                    Url = profile.ProfileUrl,
                    ThumbnailUrl = profile.AvatarUrl,
                    Footer = new EmbedFooterBuilder().WithText($"Id: {profile.Id}").WithIconUrl("https://pbs.twimg.com/media/F100zEyXwAAszNz?format=png&name=small"),
                    Timestamp = DateTimeOffset.Now,
                };

                await guildChannel.SendMessageAsync($"{trackingUser.Mention}, a cheater you're tracking has been banned:", embed: embed.Build());
            }

        }
    }
}
