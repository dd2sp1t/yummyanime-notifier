using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbSubscriptionConfig : IEntityTypeConfiguration<DbSubscription>
{
    public void Configure(EntityTypeBuilder<DbSubscription> builder)
    {
        builder.ToTable("Subscription");

        builder.HasKey(s => new { s.UserId, s.AnimeId });

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);

        // builder.HasIndex(s => new { s.UserId, s.IsDeleted });

        builder.HasIndex(s => new { s.AnimeId, s.IsDeleted });
    }
}
