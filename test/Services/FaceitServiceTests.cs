using Cs2Bot.Models;
using Cs2Bot.Services;
using Cs2Bot.Services.Interfaces;
using Cs2BotTests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json.Nodes;


namespace Cs2BotTests.Services
{
    public class FaceitServiceTests
    {
        private JsonObject sampleSteamNewsResponse;
        private Mock<IHttpClientFactory> httpClientFactoryMock;
        private Mock<IConfiguration> configMock;
        private FaceitService faceitService;
        private ThirdPartyApiMocker apiMocker;

        [SetUp]
        public void Setup()
        {
            configMock = new Mock<IConfiguration>();
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            faceitService = new FaceitService(configMock.Object, httpClientFactoryMock.Object);
            apiMocker = new ThirdPartyApiMocker();
        }

        [Test]
        public async Task GetFaceitIdFromNickname_ReturnsIdIfNicknameValid()
        {
            // Arrange
            var sampleFaceitPlayerDetailsJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitPlayerDetailsResponse.json");
            
            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleFaceitPlayerDetailsJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var faceitId = await faceitService.GetFaceitIdFromNickname("-SoS");

            // Assert
            Assert.That(String.IsNullOrEmpty(faceitId), Is.False);
            Assert.That(faceitId, Is.EqualTo("82f9f730-8e52-43c5-983f-7579b4f33672"));
        }

        
        // Temporary ban and permanent ban responses return different json from Faceit API
        // So test for both scenarios to make sure all ban types are caught
        [Test]
        public async Task GetFaceitUsersBanData_ReturnsTempBans()
        {
            // Arrange
            var faceitBanJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitTempBanResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, faceitBanJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var userIds = new List<string>() { "d7453fd9-c599-4ba6-bdb3-17f59c8814d5" };

            // Act
            List<FaceitBanData> banData = await faceitService.GetFaceitUsersBanData(userIds);

            // Assert
            Assert.That(banData, Is.Not.Null);
            Assert.That(banData, Is.TypeOf<List<FaceitBanData>>());
            Assert.That(banData[0].User_Id, Is.EqualTo(userIds[0]));
        }

        [Test]
        public async Task GetFaceitUsersBanData_ReturnsPermaBans()
        {
            // Arrange
            var faceitBanJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitPermaBanResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, faceitBanJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var userIds = new List<string>() { "b683bcaa-5070-41ee-9181-7ee34ae8c162" };

            // Act
            List<FaceitBanData> banData = await faceitService.GetFaceitUsersBanData(userIds);

            // Assert
            Assert.That(banData, Is.Not.Null);
            Assert.That(banData.Count, Is.EqualTo(userIds.Count));
            Assert.That(banData, Is.TypeOf<List<FaceitBanData>>());
            Assert.That(banData[0].User_Id, Is.EqualTo(userIds[0]));
        }

        [Test]
        public async Task GetFaceitUsersBanData_ReturnsEmptyIfNoBansFound()
        {
            // Arrange
            var faceitBanJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitEmptyBanResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, faceitBanJsonString);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var userIds = new List<string>() { "b683bcaa-5070-41ee-9181-7ee34ae8c162" };
                // Act
                List <FaceitBanData> banData = await faceitService.GetFaceitUsersBanData(userIds);

            // Assert
            Assert.That(banData, Is.Empty);
        }

        // Only called when ID has been confirmed, so no need to test scenarios where ID doesn't match a profile
        [Test]
        public async Task GetFaceitUserProfile_GetsFaceitProfile()
        {
            var sampleFaceitUserProfileJson = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitUserProfileResponse.json");

            var httpClient = apiMocker.CreateClientForMock(HttpStatusCode.OK, sampleFaceitUserProfileJson);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            CheaterProfile profile = await faceitService.GetFaceitUserProfile(It.IsAny<string>());

            // Assert
            Assert.That(profile, Is.TypeOf<CheaterProfile>());
        }
    }
}
