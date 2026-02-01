using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbUserConfig : IEntityTypeConfiguration<DbUser>
{
    public void Configure(EntityTypeBuilder<DbUser> builder)
    {
        builder.ToTable("User");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TelegramUserId)
            .IsRequired();

        builder.HasIndex(x => x.TelegramUserId)
            .IsUnique();

        builder.HasMany(x => x.Subscriptions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
        // TODO:
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
        // TODO:
            .OnDelete(DeleteBehavior.Cascade);
    }
}