using Cs2Bot.Models;
using Cs2Bot.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
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

        [SetUp]
        public void Setup()
        {
            configMock = new Mock<IConfiguration>();
            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            steamService = new SteamService(configMock.Object, httpClientFactoryMock.Object);

        }

        [Test]
        public async Task GetLatestNewsPosts_ReturnsNewsPostJson()
        {
            //Arrange
            var sampleSteamNewsJsonString = File.ReadAllText("C:\\Users\\SionS\\Code Projects\\CSBOT\\test\\Services\\SampleApiResponses\\SampleSteamNewsResponse.json");

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
                    Content = new StringContent(sampleSteamNewsJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
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
                    Content = new StringContent(sampleSteamNewsJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
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
                    Content = new StringContent(sampleSteamBansJsonString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            var steamIdsList = new List<long> { 76561198035701556, 76561198320351746, 76561198040904603 };


            // Act
            var steamBans = await steamService.GetSteamUsersBanData(steamIdsList);

            // Assert
            Assert.That(steamBans, Is.TypeOf<List<SteamUserBanData>>());
            Assert.That(steamBans.Count, Is.EqualTo(3));

        }
    }
}