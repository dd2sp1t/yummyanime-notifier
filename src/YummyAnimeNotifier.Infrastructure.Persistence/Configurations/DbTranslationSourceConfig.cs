using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbTranslationSourceConfig : IEntityTypeConfiguration<DbTranslationSource>
{
    public void Configure(EntityTypeBuilder<DbTranslationSource> builder)
    {
        builder.ToTable("TranslationSource");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Id)
            .ValueGeneratedNever();

        builder.Property(ts => ts.CreatedAt)
            .IsRequired();

        builder.Property(ts => ts.Type)
            .IsRequired();

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(ts => new { ts.Type, ts.Name })
            .IsUnique();
    }
}