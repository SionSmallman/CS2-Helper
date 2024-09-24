using System;
using System.Collections.Generic;
using Cs2Bot.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Cs2Bot.Data;

public partial class BotDbContext : DbContext
{
    public BotDbContext(DbContextOptions<BotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Guild> Guilds { get; set; }

    public virtual DbSet<GuildPatchNotesSetting> GuildPatchNotesSettings { get; set; }

    public virtual DbSet<SuspectedCheater> SuspectedCheaters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("Name=ConnectionStrings:Db", ServerVersion.Parse("8.0.30-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Guild>(entity =>
        {
            entity.HasKey(e => e.GuildId).HasName("PRIMARY");

            entity.Property(e => e.GuildId).ValueGeneratedNever();
        });

        modelBuilder.Entity<GuildPatchNotesSetting>(entity =>
        {
            entity.HasKey(e => e.GuildId).HasName("PRIMARY");

            entity.Property(e => e.GuildId).ValueGeneratedNever();

            entity.HasOne(d => d.Guild).WithOne(p => p.GuildPatchNotesSetting)
                .HasForeignKey<GuildPatchNotesSetting>(d => d.GuildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("GuildPatchNotesSettings_ibfk_1");
        });

        modelBuilder.Entity<SuspectedCheater>(entity =>
        {
            entity.HasKey(e => new { e.CheaterUserId, e.GuildId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.HasIndex(e => e.GuildId, "fk_guild_idx");

            entity.Property(e => e.CheaterUserId).HasMaxLength(45);
            entity.Property(e => e.Platform).HasColumnType("enum('Steam','Faceit')");

            entity.HasOne(d => d.Guild).WithMany(p => p.SuspectedCheaters)
                .HasForeignKey(d => d.GuildId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_guild");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
