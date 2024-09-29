using Cs2Bot.Models;
using Cs2Bot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cs2Bot.Services
{
    public class FaceitService : IFaceitService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public FaceitService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<FaceitBanData>> GetFaceitUsersBanData(List<string> playerIds)
        {
            List<FaceitBanData> bannedUsers = new List<FaceitBanData>();

            // For each supplied playerId, check Faceit API for ban data
            // If id has ban associated, add it to results list.
            foreach (var playerId in playerIds)
            {
                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, $"https://open.faceit.com/data/v4/players/{playerId}/bans");
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["FaceitApiKey"]);
                try
                {
                    using HttpClient httpClient = _httpClientFactory.CreateClient();
                    var response = httpClient.SendAsync(httpRequestMessage).Result;
                    response.EnsureSuccessStatusCode();
                    var banDataAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    JsonObject banDataObject = JsonNode.Parse(banDataAsJsonString)!.AsObject();

                    // If no bans found, go next
                    if (banDataObject["items"].AsArray().Count == 0)
                    {
                        continue;
                    }

                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    FaceitBanData banData = JsonSerializer.Deserialize<FaceitBanData>(banDataObject["items"][0].ToJsonString(), jsonOptions);
                    bannedUsers.Add(banData);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return bannedUsers;
        }

        // Gets a Faceit users player ID
        // Faceit doesn't show player IDs publicly, need to call players endpoint to obtain
        public async Task<string?> GetFaceitIdFromNickname(string faceitNickname)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://open.faceit.com/data/v4/players?nickname={faceitNickname}");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["FaceitApiKey"]);
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                if (response.IsSuccessStatusCode) 
                {
                    var playerDataAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    JsonObject playerDataObject = JsonNode.Parse(playerDataAsJsonString)!.AsObject();
                    return playerDataObject["player_id"].ToString();
                }
                else
                {
                    return null;
                }
                
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CheaterProfile?> GetFaceitUserProfile(string playerId)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://open.faceit.com/data/v4/players/{playerId}");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["FaceitApiKey"]);
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                if (response.IsSuccessStatusCode)
                {
                    var faceitUserResponseAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    JsonObject faceitUserResponseObject = JsonNode.Parse(faceitUserResponseAsJsonString)!.AsObject();

                    var userProfile = new CheaterProfile()
                    {
                        Id = playerId,
                        Nickname = faceitUserResponseObject["nickname"].ToString(),
                        AvatarUrl = faceitUserResponseObject["avatar"].ToString(),
                        ProfileUrl = $"https://faceit.com/en/players/{faceitUserResponseObject["nickname"].ToString()}"
                    };
                    return userProfile;
                }
                else
                {
                    return null;
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
