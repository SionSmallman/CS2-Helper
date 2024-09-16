using Cs2Bot.Models;
using Cs2Bot.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cs2BotTests.Services
{
    public class Tests
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
            sampleSteamNewsResponse = JsonSerializer.Deserialize<JsonObject>(sampleSteamNewsJsonString);

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
            sampleSteamNewsResponse = JsonSerializer.Deserialize<JsonObject>(sampleSteamNewsJsonString);

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
    }
}