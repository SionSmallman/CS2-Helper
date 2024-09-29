using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cs2BotTests.TestHelpers
{
    public class ThirdPartyApiMocker
    {
        public ThirdPartyApiMocker() { }
        
        // Creates a HTTP client to use when mocking a 3rd part API call
        public HttpClient CreateClientForMock(HttpStatusCode statusCode, string contentAsString)
        {
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
                    Content = new StringContent(contentAsString)
                });
            var httpClient = new HttpClient(handlerMock.Object);
            return httpClient;
        }
    }
}
