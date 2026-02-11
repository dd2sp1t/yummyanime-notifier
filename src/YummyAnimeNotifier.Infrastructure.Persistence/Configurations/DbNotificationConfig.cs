using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbNotificationConfig : IEntityTypeConfiguration<DbNotification>
{
    public void Configure(EntityTypeBuilder<DbNotification> builder)
    {
        builder.ToTable("Notification");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.UpdatedAt)
            .IsRequired(false);

        builder.Property(n => n.UserId)
            .IsRequired();

        builder.Property(n => n.ReleaseId)
            .IsRequired();

        builder.Property(n => n.AnimeName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(n => n.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.TotalEpisodes)
            .IsRequired(false);

        builder.Property(a => a.EpisodeNumber)
            .IsRequired();

        builder.Property(n => n.TranslationSourceName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(n => n.Status)
            .IsRequired();

        builder.Property(n => n.Error)
            .IsRequired(false);

        builder.HasOne(n => n.Release)
            .WithMany(r => r.Notifications)
            .HasForeignKey(n => n.ReleaseId);

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId);
    }
}