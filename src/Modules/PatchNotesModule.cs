﻿using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Cs2Bot.Models.Entities;
using Cs2Bot.Services.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using InteractionFramework;

namespace Cs2Bot.Modules
{
    public class PatchNotesModule : InteractionModuleBase<SocketInteractionContext>
    {

        private InteractionHandler _handler;
        private ISteamService _steamService;
        private IGuildPatchNotesSettingsRepository _patchSettingsRepository;

        public PatchNotesModule(InteractionHandler handler, ISteamService steamService, IGuildPatchNotesSettingsRepository patchSettingsRepository)
        {
            _handler = handler;
            _steamService = steamService;
            _patchSettingsRepository = patchSettingsRepository;
        }

        [DefaultMemberPermissions(GuildPermission.ManageChannels)]
        [SlashCommand("patchnotes", "Get live CS2 patch notes when there's a game update")]
        public async Task SetPatchChannel([ChannelTypes(ChannelType.Text), Summary(description: "The channel you want patch notes posted to")] IChannel channel)
        {
            // Cast to SocketGuildChannel to get the guild Id
            var chnl = channel as SocketGuildChannel;

            // Check if channel has already subscribed before
            // If so, update chosen channel ID
            var patchNoteRecord = await _patchSettingsRepository.GetByGuildIdAsync(chnl.Guild.Id);
            if (patchNoteRecord != null)
            {
                await _patchSettingsRepository.UpdateChannelAsync(chnl.Guild.Id, channel.Id);
                await _patchSettingsRepository.SetActiveAsync(chnl.Guild.Id);

            }
            else
            {
                var settings = new GuildPatchNotesSetting()
                {
                    GuildId = chnl.Guild.Id,
                    PatchNotesEnabled = true,
                    ChannelId = channel.Id,
                };

                await _patchSettingsRepository.CreateAsync(settings);
            }

            await RespondAsync("Channel chosen");
        }
    }
}