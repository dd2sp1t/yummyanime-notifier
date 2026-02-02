using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbAnimeConfig : IEntityTypeConfiguration<DbAnime>
{
    public void Configure(EntityTypeBuilder<DbAnime> builder)
    {
        builder.ToTable("Anime");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.SourceLink)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.OriginalName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.RuName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Year)
            .IsRequired();

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.ReleasedEpisodeCount)
            .IsRequired();

        builder.Property(a => a.TotalEpisodeCount)
            .IsRequired(false);

        builder.HasMany(a => a.Subscriptions)
            .WithOne(s => s.Anime)
            .HasForeignKey(s => s.AnimeId)
        // TODO:
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Notifications)
            .WithOne(n => n.Anime)
            .HasForeignKey(n => n.AnimeId)
        // TODO:
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.SourceLink)
            .IsUnique();
    }
}
