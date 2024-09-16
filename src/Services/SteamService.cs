using Cs2Bot.Models;
using Cs2Bot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cs2Bot.Services
{
    public class SteamService : ISteamService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public SteamService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<SteamNewsPost>> GetLatestNewsPosts(int count, int steamAppId)
        {
            var currentDateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, $"http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid={730}&count={count}&format=json&timestamp={currentDateTimestamp}");
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var newsAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JsonObject newsAsObject = JsonNode.Parse(newsAsJsonString)!.AsObject();
                var newsItemsNode = newsAsObject["appnews"]!["newsitems"];
                
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                List<SteamNewsPost> newsPosts = JsonSerializer.Deserialize<List<SteamNewsPost>>(newsItemsNode.ToJsonString(), jsonOptions);
                return newsPosts!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
