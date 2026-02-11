using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace YummyAnimeNotifier.Infrastructure.Persistence;

internal class YummyAnimeDbContext : DbContext
{
    public DbSet<DbAnime> Anime => Set<DbAnime>();
    public DbSet<DbAnimeTranslation> AnimeTranslations => Set<DbAnimeTranslation>();
    public DbSet<DbTranslationSource> TranslationSources => Set<DbTranslationSource>();
    public DbSet<DbRelease> Releases => Set<DbRelease>();
    public DbSet<DbUser> Users => Set<DbUser>();
    public DbSet<DbSubscription> Subscriptions => Set<DbSubscription>();
    public DbSet<DbNotification> Notifications => Set<DbNotification>();
    public DbSet<DbOutboxMessage> OutboxMessages => Set<DbOutboxMessage>();

    public YummyAnimeDbContext(DbContextOptions<YummyAnimeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YummyAnimeDbContext).Assembly);
    }
}
