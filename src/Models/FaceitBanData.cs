namespace Cs2Bot.Models
{
    // See here for Faceit api docs: https://docs.faceit.com/docs/data-api/data/#tag/Players/operation/getPlayerBans
    public class FaceitBanData
    {
        public string User_Id { get; set; }
        public string? Ends_At { get; set; }  //DateTime encoded as string (e.g "2019-08-24T14:15:22Z")
        public string? Game { get; set; }
        public string Nickname { get; set; }
        public string Reason { get; set; }
        public string Starts_At { get; set; } //DateTime encoded as string (e.g "2019-08-24T14:15:22Z")
        public string Type { get; set; }
        
    }
}
