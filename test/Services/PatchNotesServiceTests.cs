using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Cs2Bot.Services;
using Cs2Bot.Services.Interfaces;
using Discord.WebSocket;
using Moq;
using System.Text.Json.Nodes;

namespace Cs2BotTests.Services
{
    public class PatchNoteServiceTests
    {
        private JsonObject sampleSteamNewsResponse;
        private Mock<ISteamService> steamServiceMock;
        private Mock<IGuildPatchNotesSettingsRepository> patchNotesSettingRepositoryMock;
        private Mock<DiscordSocketClient> discordSocketClientMock;
        private PatchNotesService patchNotesService;

        [SetUp]
        public void Setup()
        {
            steamServiceMock = new Mock<ISteamService>();
            patchNotesSettingRepositoryMock = new Mock<IGuildPatchNotesSettingsRepository>();
            discordSocketClientMock = new Mock<DiscordSocketClient>();
            patchNotesService = new PatchNotesService(steamServiceMock.Object, patchNotesSettingRepositoryMock.Object, discordSocketClientMock.Object);
        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_IgnoresOldPatchNotes()
        {
            // Arrange
            var outdatedPatchNotesNewsPost = new SteamNewsPost()
            {
                GId = "test",
                Title = "test",
                Url = "test",
                Author = "test",
                Contents = "test",
                FeedLabel = "test",
                Feed_Type = 1,
                AppId = 730,
                Tags = ["patchnotes"],
                Date = 0 //Old timestamp
            };

            var returnList = new List<SteamNewsPost> { outdatedPatchNotesNewsPost };

            steamServiceMock.Setup(x => x.GetLatestNewsPosts(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(returnList);

            // Act
            var result = await patchNotesService.CheckForNewPatchNotesAsync();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_ReturnsNullIfNoPatchNotesFound()
        {
            // Arrange
            var noPatchNotesNewsPost = new SteamNewsPost()
            {
                GId = "test",
                Title = "test",
                Url = "test",
                Author = "test",
                Contents = "test",
                FeedLabel = "test",
                Feed_Type = 1,
                AppId = 730,
                Tags = [], //No patchnote tags
                Date = DateTimeOffset.Now.ToUnixTimeSeconds() 
            };

            var returnList = new List<SteamNewsPost> { noPatchNotesNewsPost };

            steamServiceMock.Setup(x => x.GetLatestNewsPosts(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(returnList);

            // Act
            var result = await patchNotesService.CheckForNewPatchNotesAsync();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_ReturnsValidPatchNotesIfFound()
        {
            // Arrange
            var validPatchNotesNewsPost = new SteamNewsPost()
            {
                GId = "test",
                Title = "test",
                Url = "test",
                Author = "test",
                Contents = "test",
                FeedLabel = "test",
                Feed_Type = 1,
                AppId = 730,
                Tags = ["patchnotes"],
                Date = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            var returnList = new List<SteamNewsPost> { validPatchNotesNewsPost };

            steamServiceMock.Setup(x => x.GetLatestNewsPosts(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(returnList);

            // Act
            var result = await patchNotesService.CheckForNewPatchNotesAsync();

            // Assert
            Assert.That(result, Is.TypeOf<SteamNewsPost>());
        }

        [Test]
        public async Task CheckForNewPatchNotesAsync_ReturnsNewestPatchNotes()
        {
            // Arrange
            var correctPatchNotesNewsPost = new SteamNewsPost()
            {
                GId = "test",
                Title = "Most Recent",
                Url = "test",
                Author = "test",
                Contents = "test",
                FeedLabel = "test",
                Feed_Type = 1,
                AppId = 730,
                Tags = ["patchnotes"],
                Date = 77 // Most recent patch notes
            };

            var validPatchNotesNewsPost = new SteamNewsPost()
            {
                GId = "test",
                Title = "test",
                Url = "test",
                Author = "test",
                Contents = "test",
                FeedLabel = "test",
                Feed_Type = 1,
                AppId = 730,
                Tags = ["patchnotes"],
                Date = 66
            };

            var validPatchNotesNewsPost2 = new SteamNewsPost()
            {
                GId = "test",
                Title = "test",
                Url = "test",
                Author = "test",
                Contents = "test",
                FeedLabel = "test",
                Feed_Type = 1,
                AppId = 730,
                Tags = [],
                Date = 99
            };
            var oldPatchNoteNewsPost = new Mock<SteamNewsPost>();
            oldPatchNoteNewsPost.SetupAllProperties();
            oldPatchNoteNewsPost.Object.Tags = ["patchnotes"];


            var returnList = new List<SteamNewsPost> { correctPatchNotesNewsPost, validPatchNotesNewsPost, validPatchNotesNewsPost2};

            steamServiceMock.Setup(x => x.GetLatestNewsPosts(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(returnList);

            // Act
            var result = await patchNotesService.CheckForNewPatchNotesAsync();

            // Assert
            Assert.That(result, Is.TypeOf<SteamNewsPost>());
            Assert.That(result.Title, Is.EqualTo("Most Recent"));
        }
    }
}