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
    public class OnJoinService
    {
        private DiscordSocketClient _client;
        private IGuildRepository _guildRepository;

        public OnJoinService(DiscordSocketClient client, IGuildRepository guildRepository)
        {
            _client = client;
            _guildRepository = guildRepository;
            _client.JoinedGuild += JoinedGuild;
            _client.LeftGuild += LeftGuild;
        }

        private async Task<Task> JoinedGuild(SocketGuild guild)
        {
            Console.WriteLine($"Joined {guild.Name}, GuildId: {guild.Id}");

            // If we have previously been in this guild, just set as active
            // If not, create new entry in Guilds table
            var checkIfPreviousGuild = await _guildRepository.GetByIdAsync(guild.Id);
            if (checkIfPreviousGuild != null)
            {
                await _guildRepository.SetGuildAsActiveAsync(guild.Id);
                return Task.CompletedTask;
            }
            
            var dbGuild = new Guild()
            {
                GuildId = guild.Id,
                IsActive = true
            };

            await _guildRepository.CreateAsync(dbGuild);

            return Task.CompletedTask;
        }

        private async Task<Task> LeftGuild(SocketGuild guild)
        {
            Console.WriteLine($"Left {guild.Name}, GuildId: {guild.Id}");
            await _guildRepository.SetGuildAsInactiveAsync(guild.Id);
            return Task.CompletedTask;
        }
    }
}
