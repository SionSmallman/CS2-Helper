using Cs2Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs2Bot.Data.Repositories
{
    public class PatchNoteSettingRepository
    {
        private readonly BotDbContext _context;

        public PatchNoteSettingRepository(BotDbContext context)
        {
            _context = context;
        }


    }
}
