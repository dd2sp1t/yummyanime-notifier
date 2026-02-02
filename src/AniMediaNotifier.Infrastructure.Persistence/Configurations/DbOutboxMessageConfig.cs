using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AniMediaNotifier.Infrastructure.Persistence.Configurations;

public class DbOutboxMessageConfig : IEntityTypeConfiguration<DbOutboxMessage>
{
    public void Configure(EntityTypeBuilder<DbOutboxMessage> builder)
    {
        builder.ToTable("OutboxMessage");

        builder.HasKey(m => m.Id);
    }
}