using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Configurations;

public class DbOutboxMessageConfig : IEntityTypeConfiguration<DbOutboxMessage>
{
    public void Configure(EntityTypeBuilder<DbOutboxMessage> builder)
    {
        builder.ToTable("OutboxMessage");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .IsRequired(false);

        builder.Property(m => m.EventType)
            .IsRequired();

        builder.Property(m => m.Payload)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.Error)
            .IsRequired(false);
    }
}