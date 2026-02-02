using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbNotificationConfig : IEntityTypeConfiguration<DbNotification>
{
    public void Configure(EntityTypeBuilder<DbNotification> builder)
    {
        builder.ToTable("Notification");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever();

        builder.Property(n => n.UserId)
            .IsRequired();

        builder.Property(n => n.AnimeId)
            .IsRequired();

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(n => n.IsSent)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.SentAt)
            .IsRequired(false);

        // TODO:
        // builder.HasIndex(n => n.UserId);
        // builder.HasIndex(n => n.AnimeId);
        // builder.HasIndex(n => new { n.UserId, n.IsSent });
    }
}