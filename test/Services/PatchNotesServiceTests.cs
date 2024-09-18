using Cs2Bot.Data.Repositories;
using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Cs2Bot.Services;
using Cs2Bot.Services.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cs2BotTests.Services
{
    public class PatchNoteServiceTests
    {
        private JsonObject sampleSteamNewsResponse;
        private Mock<ISteamService> steamServiceMock;
        private Mock<IPatchNotesSettingRepository> patchNotesSettingRepositoryMock;
        private Mock<DiscordSocketClient> discordSocketClientMock;
        private PatchNotesService patchNotesService;

        [SetUp]
        public void Setup()
        {
            steamServiceMock = new Mock<ISteamService>();
            patchNotesSettingRepositoryMock = new Mock<IPatchNotesSettingRepository>();
            discordSocketClientMock = new Mock<DiscordSocketClient>();
            patchNotesService = new PatchNotesService(steamServiceMock.Object, patchNotesSettingRepositoryMock.Object, discordSocketClientMock.Object);
        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_IgnoresOldPatchNotes()
        {

        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_ReturnsNullIfNoPatchNotesFound()
        {

        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_ReturnsNewestPatchNotes()
        {

        }
    }
}