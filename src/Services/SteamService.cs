using Cs2Bot.Models;
using Cs2Bot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
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

        // Get the latest news posts for a given Steam app
        public async Task<List<SteamNewsPost>> GetLatestNewsPosts(int count, int steamAppId)
        {
            // Timestamp API request to ensure data isn't cached
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

        // GetPlayerBans endpoint supports searching with comma seperated list of SteamIDs 
        // So search as list to save API calls in event that more than one ID needs searching
        public async Task<List<SteamUserBanData>> GetSteamUsersBanData(List<string> steamId64List)
        {
            List<SteamUserBanData> bannedUsers = new();

            // If no id's provided, early return the empty list
            if (steamId64List.Count == 0) { return bannedUsers; }
            
            // Convert list of IDs into comma seperated string
            var steamIdListAsString = String.Join(",", steamId64List);
            
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get, $"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={_config["SteamApiKey"]}&steamids={steamIdListAsString}");
            
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var banDataAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JsonObject banDataAsObject = JsonNode.Parse(banDataAsJsonString)!.AsObject();

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };               
                
                // Steam returns a ban object even when the account isn't banned, so filter out the banned users and return that list
                // If no valid bans are found, return an empty list
                List<SteamUserBanData> steamUserBansDataList = JsonSerializer.Deserialize<List<SteamUserBanData>>(banDataAsObject["players"]!.ToJsonString(), jsonOptions);

                var validBans = steamUserBansDataList.Where(x => x.VACBanned == true).ToList();
                if (validBans != null) 
                {
                    bannedUsers.AddRange(validBans);
                }
                return bannedUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Gets a Steam users profile details
        public async Task<CheaterProfile> GetSteamUserProfile(string playerId)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_config["SteamApiKey"]}&steamids={playerId}");
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var steamUserResponseAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JsonObject steamUserResponseObject = JsonNode.Parse(steamUserResponseAsJsonString)!.AsObject();
                var userNode = steamUserResponseObject["response"]["players"]![0];

                var userProfile = new CheaterProfile()
                {
                    Id = playerId,
                    Nickname = userNode["personaname"].ToString(),
                    AvatarUrl = userNode["avatarmedium"].ToString(),
                    ProfileUrl = userNode["profileurl"].ToString()
                };
                return userProfile;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Resolves custom vanity URL to SteamId64
        public async Task<string> GetSteamId64FromVanityUrl(string vanityUrl)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={_config["SteamApiKey"]}&vanityurl={vanityUrl}");
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var steamResponseAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(steamResponseAsJsonString);
                JsonObject steamResponseObject = JsonNode.Parse(steamResponseAsJsonString)!.AsObject();
                return steamResponseObject["response"]["steamid"].ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
