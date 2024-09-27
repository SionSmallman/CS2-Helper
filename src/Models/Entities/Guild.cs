namespace Cs2Bot.Models.Entities;

public partial class Guild
{
    public ulong GuildId { get; set; }

    public bool IsActive { get; set; }

    public virtual GuildPatchNotesSetting? GuildPatchNotesSetting { get; set; }

    public virtual ICollection<SuspectedCheater> SuspectedCheaters { get; set; } = new List<SuspectedCheater>();
}
