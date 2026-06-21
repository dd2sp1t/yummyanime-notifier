using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbAnimeTranslationConfig : IEntityTypeConfiguration<DbAnimeTranslation>
{
    public void Configure(EntityTypeBuilder<DbAnimeTranslation> builder)
    {
        builder.ToTable("AnimeTranslation");

        builder.HasKey(t => new { t.AnimeId, t.TranslationSourceId });

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .IsRequired();

        builder.Property(t => t.TotalEpisodes)
            .IsRequired(false);

        builder.Property(t => t.ReleasedEpisodes)
            .IsRequired();

        builder.HasOne(t => t.Anime)
            .WithMany(a => a.AnimeTranslations)
            .HasForeignKey(t => t.AnimeId);

        builder.HasOne(t => t.TranslationSource)
            .WithMany(ts => ts.AnimeTranslations)
            .HasForeignKey(t => t.TranslationSourceId);
    }
}