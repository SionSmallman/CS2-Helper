using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Cs2Bot.Models.Entities;
using Cs2Bot.Services;
using Cs2Bot.Services.Interfaces;
using Discord.WebSocket;  
using Moq;


namespace Cs2BotTests.Services
{
    internal class SuspectedCheaterServiceTests
    {
        private Mock<ISuspectedCheatersRepository> suspectRepositoryMock;
        private Mock<ISteamService> steamServiceMock; 
        private Mock<IFaceitService> faceitServiceMock;
        private Mock<DiscordSocketClient> discordSocketClientMock;
        private SuspectedCheaterService suspectedCheaterService;

        [SetUp]
        public void Setup()
        {
            faceitServiceMock = new Mock<IFaceitService>();
            steamServiceMock = new Mock<ISteamService>();
            suspectRepositoryMock = new Mock<ISuspectedCheatersRepository>();
            discordSocketClientMock = new Mock<DiscordSocketClient>();
            suspectedCheaterService = new SuspectedCheaterService(suspectRepositoryMock.Object, faceitServiceMock.Object, steamServiceMock.Object, discordSocketClientMock.Object);
        }

        [Test]
        public async Task CheckForNewBansAsync_ReturnsEmptyIfNoUnbannedSuspects()
        {
            // Arrange
            suspectRepositoryMock.Setup(x => x.GetAllUnbannedCheaters()).ReturnsAsync(new List<SuspectedCheater>());
            steamServiceMock.Setup(x => x.GetSteamUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<SteamUserBanData>());
            faceitServiceMock.Setup(x => x.GetFaceitUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<FaceitBanData>());

            // Act
            var newBans = await suspectedCheaterService.CheckForNewBansAsync();

            // Assert
            Assert.That(newBans, Is.Empty);
        }

        [Test]
        public async Task CheckForNewBansAsync_ReturnsEmptyIfNoNewBans()
        {
            // Arrange
            var testSuspect = new SuspectedCheater()
            {
                CheaterUserId = "test",
                Platform = "Steam",
                IsBanned = false,
                GuildId = 98127364,
                DiscordUserId = 2903845,
                ChannelId = 039845,
            };

            suspectRepositoryMock.Setup(x => x.GetAllUnbannedCheaters()).ReturnsAsync(new List<SuspectedCheater>() { testSuspect });
            steamServiceMock.Setup(x => x.GetSteamUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<SteamUserBanData>());
            faceitServiceMock.Setup(x => x.GetFaceitUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<FaceitBanData>());

            // Act
            var newBans = await suspectedCheaterService.CheckForNewBansAsync();

            // Assert
            Assert.That(newBans, Is.Empty);
        }

        [Test]
        public async Task CheckForNewBansAsync_ReturnsValidSteamBans()
        {
            // Arrange
            var testSuspect = new SuspectedCheater()
            {
                CheaterUserId = "test",
                Platform = "Steam",
                IsBanned = false,
                GuildId = 98127364,
                DiscordUserId = 2903845,
                ChannelId = 039845,
            };

            var validSteamBan = new SteamUserBanData()
            {
                SteamId = "test",
                CommunityBanned = false,
                VACBanned = true,
                NumberOfVACBans = 1,
                NumberOfGameBans = 0,
                DaysSinceLastBan = 1,
                EconomyBan = "none"
            };

            suspectRepositoryMock.Setup(x => x.GetAllUnbannedCheaters()).ReturnsAsync(new List<SuspectedCheater>() { testSuspect });
            steamServiceMock.Setup(x => x.GetSteamUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<SteamUserBanData>() { validSteamBan });
            faceitServiceMock.Setup(x => x.GetFaceitUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<FaceitBanData>());

            // Act
            var newBans = await suspectedCheaterService.CheckForNewBansAsync();

            // Assert
            Assert.That(newBans.Count, Is.EqualTo(1));
            Assert.That(newBans[0].CheaterUserId, Is.EqualTo(testSuspect.CheaterUserId));
        }

        [Test]
        public async Task CheckForNewBansAsync_ReturnsValidFaceitBans()
        {
            // Arrange
            var testSuspect = new SuspectedCheater()
            {
                CheaterUserId = "test",
                Platform = "Faceit",
                IsBanned = false,
                GuildId = 98127364,
                DiscordUserId = 2903845,
                ChannelId = 039845,
            };

            var validFaceitBan = new FaceitBanData()
            {
                User_Id = "test",
                Ends_At = "2019-08-24T14:15:22Z",
                Game = "cs2",
                Nickname = "test",
                Reason = "test",
                Starts_At = "2019-08-24T14:15:22Z",
                Type = "test",
            };

            suspectRepositoryMock.Setup(x => x.GetAllUnbannedCheaters()).ReturnsAsync(new List<SuspectedCheater>() { testSuspect });
            steamServiceMock.Setup(x => x.GetSteamUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<SteamUserBanData>());
            faceitServiceMock.Setup(x => x.GetFaceitUsersBanData(It.IsAny<List<string>>())).ReturnsAsync(new List<FaceitBanData>() { validFaceitBan });

            // Act
            var newBans = await suspectedCheaterService.CheckForNewBansAsync();

            // Assert
            Assert.That(newBans.Count, Is.EqualTo(1));
            Assert.That(newBans[0].CheaterUserId, Is.EqualTo(testSuspect.CheaterUserId));
        }
    }
}
