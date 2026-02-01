using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbSubscriptionConfig : IEntityTypeConfiguration<DbSubscription>
{
    public void Configure(EntityTypeBuilder<DbSubscription> builder)
    {
        builder.ToTable("Subscription");

        builder.HasKey(x => new { x.UserId, x.AnimeId });

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        // builder.HasIndex(x => new { x.UserId, x.IsDeleted });

        builder.HasIndex(x => new { x.AnimeId, x.IsDeleted });
    }
}
