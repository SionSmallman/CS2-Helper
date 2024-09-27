using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Models
{
    public class FaceitUser
    {
        public required string PlayerId { get; set; }
        public required string Nickname { get; set; }
        public string Avatar { get; set; }
        public long SteamId { get; set; }

    }
}
