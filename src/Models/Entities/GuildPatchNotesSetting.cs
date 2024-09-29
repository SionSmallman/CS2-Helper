namespace Cs2Bot.Models.Entities;

public partial class GuildPatchNotesSetting
{
    public ulong GuildId { get; set; }

    public bool PatchNotesEnabled { get; set; }

    public ulong? ChannelId { get; set; }

    public virtual Guild Guild { get; set; } = null!;
}
