using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbAnimeConfig : IEntityTypeConfiguration<DbAnime>
{
    public void Configure(EntityTypeBuilder<DbAnime> builder)
    {
        builder.ToTable("Anime");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceLink)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.OriginalName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.RuName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.ReleasedEpisodeCount)
            .IsRequired();

        builder.Property(x => x.TotalEpisodeCount)
            .IsRequired(false);

        builder.HasMany(x => x.Subscriptions)
            .WithOne(x => x.Anime)
            .HasForeignKey(x => x.AnimeId)
        // TODO:
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Notifications)
            .WithOne(n => n.Anime)
            .HasForeignKey(n => n.AnimeId)
        // TODO:
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SourceLink)
            .IsUnique();
    }
}
