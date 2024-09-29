using Cs2Bot.Models;
using Cs2Bot.Services;
using Cs2BotTests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cs2BotTests.Services
{
    public class SteamServiceTests
    {
        private JsonObject sampleSteamNewsResponse;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<IConfiguration> configMock;
        private SteamService steamService;
        private ThirdPartyApiMocker apiMocker;

        [SetUp]
        public void Setup()
        {
            configMock = new Mock<IConfiguration>();
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            steamService = new SteamService(configMock.Object, httpClientFactoryMock.Object);
            apiMocker = new ThirdPartyApiMocker();
        }

        [Test]
        public async Task GetLatestNewsPosts_ReturnsNewsPostJson()
        {
            //Arrange
            var sampleSteamNewsJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\SampleSteamNewsResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleSteamNewsJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var newsPosts = await steamService.GetLatestNewsPosts(3, 730);

            // Assert
            Assert.That(newsPosts, Is.Not.Null);
            Assert.That(newsPosts, Is.TypeOf<List<SteamNewsPost>>());
        }

        [Test]
        public async Task GetLatestNewsPosts_ReturnsEmptyIfNoPosts()
        {
            //Arrange
            var sampleSteamNewsJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\EmptySteamNewsResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleSteamNewsJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var newsPosts = await steamService.GetLatestNewsPosts(3, 730);

            // Assert
            Assert.That(newsPosts, Is.TypeOf<List<SteamNewsPost>>());
            Assert.That(newsPosts.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSteamUsersBanData_GetsMultipleSteamIds()
        {
            // Arrange
            var sampleSteamBansJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\SteamUserBanResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleSteamBansJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var steamIdsList = new List<string> { "76561198035701556", "76561198320351746", "76561198040904603" };

            // Act
            var steamBans = await steamService.GetSteamUsersBanData(steamIdsList);

            // Assert
            Assert.That(steamBans, Is.TypeOf<List<SteamUserBanData>>());
            Assert.That(steamBans.Count, Is.EqualTo(1));
        }



        [Test]
        public async Task GetSteamId64FromVanityUrl_ReturnsValidSteamId64()
        {
            // Arrange
            var sampleSteamVanityResolveJson = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\SteamVanityUrlResolveResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleSteamVanityResolveJson);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var steamId64 = await steamService.GetSteamId64FromVanityUrl(It.IsAny<string>());

            // Assert
            Assert.That(String.IsNullOrEmpty(steamId64), Is.False);
        }

        [Test]
        public async Task GetSteamId64FromVanityUrl_ReturnsNullIfNotFound()
        {
            // Arrange
            var sampleSteamVanityResolveJson = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\EmptySteamVanityUrlResolveResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleSteamVanityResolveJson);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var steamId64 = await steamService.GetSteamId64FromVanityUrl(It.IsAny<string>());

            // Assert
            Assert.That(String.IsNullOrEmpty(steamId64), Is.True);
        }

        // Only called when ID has been confirmed, so no need to test scenarios where ID doesn't match a profile
        [Test]
        public async Task GetSteamUserProfile_ReturnsProfile()
        {
            var sampleSteamUserProfileJson = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\SteamUserProfileResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleSteamUserProfileJson);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            CheaterProfile profile = await steamService.GetSteamUserProfile(It.IsAny<string>());

            // Assert
            Assert.That(profile, Is.TypeOf<CheaterProfile>());
        }
    }
}