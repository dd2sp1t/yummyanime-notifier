using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbSubscriptionConfig : IEntityTypeConfiguration<DbSubscription>
{
    public void Configure(EntityTypeBuilder<DbSubscription> builder)
    {
        builder.ToTable("Subscription");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.AnimeId)
            .IsRequired();

        builder.Property(s => s.TranslationSourceId)
            .IsRequired(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(s => s.Anime)
            .WithMany(a => a.Subscriptions)
            .HasForeignKey(s => s.AnimeId);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId);

        builder.HasOne(s => s.TranslationSource)
            .WithMany(ts => ts.Subscriptions)
            .HasForeignKey(s => s.TranslationSourceId);

        builder.HasIndex(s => new { s.UserId, s.AnimeId, s.TranslationSourceId })
            .IsUnique();
    }
}
