using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbAnimeConfig : IEntityTypeConfiguration<DbAnime>
{
    public void Configure(EntityTypeBuilder<DbAnime> builder)
    {
        builder.ToTable("Anime");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);

        builder.Property(a => a.ExternalId)
            .IsRequired();

        builder.Property(a => a.SourceLink)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        // TODO:
        builder.Property(a => a.ReleasedEpisodes)
            .IsRequired(false);

        builder.Property(a => a.TotalEpisodes)
            .IsRequired(false);

        builder.HasMany(a => a.Releases)
            .WithOne(r => r.Anime)
            .HasForeignKey(r => r.AnimeId);

        builder.HasMany(a => a.AnimeTranslations)
            .WithOne(l => l.Anime)
            .HasForeignKey(l => l.AnimeId);

        builder.HasMany(a => a.Subscriptions)
            .WithOne(s => s.Anime)
            .HasForeignKey(s => s.AnimeId);

        builder.HasIndex(a => a.ExternalId)
            .IsUnique();

        builder.HasIndex(a => a.SourceLink)
            .IsUnique();
    }
}
