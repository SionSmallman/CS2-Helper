namespace Cs2Bot.Models
{
    public class SteamUserBanData
    {
        public required string SteamId { get; set; }
        public required bool CommunityBanned { get; set; }
        public required bool VACBanned { get; set; }
        public required int NumberOfVACBans { get; set; }
        public required int DaysSinceLastBan { get; set; }
        public required int NumberOfGameBans { get; set; }
        
        // Instead of using a bool, the Steam API decided to use strings, either "none" or "banned"...
        public required string EconomyBan { get; set; } 
    }
}
