using Cs2Bot.Models;
using Cs2Bot.Services;
using Cs2Bot.Services.Interfaces;
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

        [SetUp]
        public void Setup()
        {
            configMock = new Mock<IConfiguration>();
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            faceitService = new FaceitService(configMock.Object, httpClientFactoryMock.Object);
        }

        [Test]
        public async Task GetFaceitIdFromNickname_ReturnsIdIfNicknameValid()
        {
            // Arrange
            var sampleFaceitPlayerDetailsJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitPlayerDetailsResponse.json");

            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(sampleFaceitPlayerDetailsJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
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
        public async Task GetFaceitBanData_ReturnsTempBans()
        {
            // Arrange
            var faceitBanJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitTempBanResponse.json");

            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(faceitBanJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var userId = "d7453fd9-c599-4ba6-bdb3-17f59c8814d5";

            // Act
            List<FaceitBanData> banData = await faceitService.GetFaceitUserBanData(userId);

            // Assert
            Assert.That(banData, Is.Not.Null);
            Assert.That(banData, Is.TypeOf<List<FaceitBanData>>());
            Assert.That(banData[0].User_Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetFaceitBanData_ReturnsPermaBans()
        {
            // Arrange
            var faceitBanJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitPermaBanResponse.json");

            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(faceitBanJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var userId = "b683bcaa-5070-41ee-9181-7ee34ae8c162";

            // Act
            List<FaceitBanData> banData = await faceitService.GetFaceitUserBanData(userId);

            // Assert
            Assert.That(banData, Is.Not.Null);
            Assert.That(banData, Is.TypeOf<List<FaceitBanData>>());
            Assert.That(banData[0].User_Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetFaceitBanData_ReturnsNullIfNoBansFound()
        {
            // Arrange
            var faceitBanJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\FaceitEmptyBanResponse.json");

            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(faceitBanJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var userId = "b683bcaa-5070-41ee-9181-7ee34ae8c162";

            // Act
            List<FaceitBanData> banData = await faceitService.GetFaceitUserBanData(userId);

            // Assert
            Assert.That(banData, Is.Null);
        }
    }
}
