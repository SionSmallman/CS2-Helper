using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Helpers;
using Cs2Bot.Models;
using Cs2Bot.Services.Interfaces;
using Discord;
using Discord.WebSocket;

namespace Cs2Bot.Services
{
    public class PatchNotesService : IPatchNotesService
    {
        private readonly ISteamService _steamService;
        private readonly IGuildPatchNotesSettingsRepository _patchNotesSettingRepository;
        private readonly DiscordSocketClient _client;
        private long _lastUpdateTimestamp = 0; 

        public PatchNotesService(ISteamService steamService, IGuildPatchNotesSettingsRepository patchNotesSettingRepository, DiscordSocketClient client)
        {
            _steamService = steamService;
            _patchNotesSettingRepository = patchNotesSettingRepository;
            _client = client;
        }

        public async Task<SteamNewsPost> CheckForNewPatchNotesAsync()
        {
            Console.WriteLine("Checking for new patch notes. Time Checked: " + DateTime.Now.ToString());
            Console.WriteLine(_lastUpdateTimestamp);
            List<SteamNewsPost> latestPosts  = await _steamService.GetLatestNewsPosts(3, 730);

            var helper = new PatchNotesHelper();
            
            // Get patch note with largest timestamp
            // If no patchnotes post found, return null
            var latestPatchNotes = latestPosts.Where(x => x.Tags.Contains("patchnotes")).MaxBy(x => x.Date);
            if (latestPatchNotes == null)
            {
                return null;
            }

            // If post timestamp is smaller or equal to current latest, return early.
            if (latestPatchNotes.Date <= _lastUpdateTimestamp)
            {
                return null;
            }
            _lastUpdateTimestamp = latestPatchNotes.Date;

            Console.WriteLine($"New patch notes found! \n Title: {latestPatchNotes.Title} \nDate: {DateTimeOffset.FromUnixTimeSeconds(_lastUpdateTimestamp).ToString()}");

            return latestPatchNotes;
        }

        public async Task SendPatchNotesToSubscribedGuilds(SteamNewsPost patchNotesPost)
        {

            // Replace current contents with discord friendly formatted contents
            var formattedPostContents = new PatchNotesHelper().ParseContent(patchNotesPost.Contents);
            patchNotesPost.Contents = formattedPostContents;

            // Get all records of guilds subscribed to patch notes
            var patchNoteRecords = await _patchNotesSettingRepository.GetAllAsync();
            var subscribedRecords = patchNoteRecords.Where(x => x.PatchNotesEnabled);

            var embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder().WithName("Counter Strike 2 - New Patch"),
                Color = Color.Gold,
                Title = patchNotesPost.Title,
                Url = patchNotesPost.Url,
                ThumbnailUrl = "https://pbs.twimg.com/media/F100zEyXwAAszNz?format=png&name=small",
                Description = patchNotesPost.Contents,
                Footer = new EmbedFooterBuilder().WithText("Update date/time:").WithIconUrl("https://pbs.twimg.com/media/F100zEyXwAAszNz?format=png&name=small"),
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(patchNotesPost.Date)
            };

            // Send embed to all subscribed guilds
            foreach (var record in subscribedRecords) {
                var channel = _client.GetChannel((ulong)record.ChannelId!) as IMessageChannel;
                if (channel == null)
                {
                    Console.WriteLine("Patch Note Channel not found. GuildID: " + record.GuildId);
                    continue;
                }
                await channel.SendMessageAsync("New CS2 patch notes posted!", embed: embed.Build());
            };
        }
    }
}
