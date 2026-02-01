using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbNotificationConfig : IEntityTypeConfiguration<DbNotification>
{
    public void Configure(EntityTypeBuilder<DbNotification> builder)
    {
        builder.ToTable("Notification");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.AnimeId)
            .IsRequired();

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(x => x.IsSent)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.SentAt)
            .IsRequired(false);

        // TODO:
        // builder.HasIndex(x => x.UserId);
        // builder.HasIndex(x => x.AnimeId);
        // builder.HasIndex(x => new { x.UserId, x.IsSent });
    }
}