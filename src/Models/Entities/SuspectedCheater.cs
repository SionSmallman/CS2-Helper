namespace Cs2Bot.Models.Entities;

public partial class SuspectedCheater
{
    public string CheaterUserId { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public bool IsBanned { get; set; }

    public ulong GuildId { get; set; }

    public ulong DiscordUserId { get; set; }

    public ulong ChannelId { get; set; }

    public virtual Guild Guild { get; set; } = null!;
}
