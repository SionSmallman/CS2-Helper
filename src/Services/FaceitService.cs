using Cs2Bot.Models;
using Cs2Bot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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

        public async Task<List<FaceitBanData>?> GetFaceitUserBanData(string playerId)
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

                // If no bans found, return null
                if (banDataObject["items"].AsArray().Count == 0)
                {
                    return null;
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                List<FaceitBanData> banData = JsonSerializer.Deserialize<List<FaceitBanData>>(banDataObject["items"].ToJsonString(), jsonOptions);
                return banData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Faceit doesn't show player IDs publically, need to call players endpoint to obtain
        public async Task<string> GetFaceitIdFromNickname(string faceitNickname)
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get, $"https://open.faceit.com/data/v4/players?nickname={faceitNickname}");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config["FaceitApiKey"]);
            try
            {
                using HttpClient httpClient = _httpClientFactory.CreateClient();
                var response = httpClient.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                var playerDataAsJsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JsonObject playerDataObject = JsonNode.Parse(playerDataAsJsonString)!.AsObject();
                return playerDataObject["player_id"].ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
