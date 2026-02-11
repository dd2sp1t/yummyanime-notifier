using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbReleaseConfig : IEntityTypeConfiguration<DbRelease>
{
    public void Configure(EntityTypeBuilder<DbRelease> builder)
    {
        builder.ToTable("Release");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.AnimeId)
            .IsRequired();

        builder.Property(r => r.TranslationSourceId)
            .IsRequired();

        builder.Property(r => r.EpisodeNumber)
            .IsRequired();

        builder.HasOne(r => r.Anime)
            .WithMany(e => e.Releases)
            .HasForeignKey(r => r.AnimeId);

        builder.HasOne(r => r.TranslationSource)
            .WithMany(ts => ts.Releases)
            .HasForeignKey(r => r.TranslationSourceId);

        builder.HasMany(r => r.Notifications)
            .WithOne(n => n.Release)
            .HasForeignKey(n => n.ReleaseId);

        builder.HasIndex(r => new { r.AnimeId, r.TranslationSourceId, r.EpisodeNumber })
            .IsUnique();
    }
}