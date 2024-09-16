using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Discord.Interactions;
using Discord.WebSocket;
using InteractionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Services
{
    public class OnJoin
    {
        private DiscordSocketClient _client;
        private IGuildRepository _guildRepository;

        public OnJoin(DiscordSocketClient client, IGuildRepository guildRepository)
        {
            _client = client;
            _guildRepository = guildRepository;
        }

        public async Task OnJoinAsync(SocketGuild guild)
        {
            Console.WriteLine($"Joined {guild.Name}, GuildId: {guild.Id}");
            var dbGuild = new Guild()
            {
                GuildId = (long)guild.Id,
                IsActive = true
            };
            
            
            await _guildRepository.CreateAsync(dbGuild);

            return;
        }
    }
}
