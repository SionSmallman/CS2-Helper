using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Models
{
    public class SteamNewsPost
    {
        public required string GId {  get; set; }
        public required string Title { get; set; }
        public required string Url { get; set; }
        public bool Is_External_Url { get; set; } // Underscores to match json response for easy deserialization
        public required string Author { get; set; }
        public required string Contents { get; set; }
        public required string FeedLabel { get; set; }
        public required long Date { get; set; } // Unix Timestamp
        public required int Feed_Type { get; set; }
        public required int AppId { get; set; }
        public List<string> Tags { get; set; }

    }
}
