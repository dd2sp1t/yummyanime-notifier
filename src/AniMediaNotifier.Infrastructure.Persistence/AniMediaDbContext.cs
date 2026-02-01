using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Infrastructure.Persistence;

    internal class AniMediaDbContext : DbContext
    {
        public DbSet<DbAnime> Animes => Set<DbAnime>();
        public DbSet<DbUser> Users => Set<DbUser>();
        public DbSet<DbSubscription> Subscriptions => Set<DbSubscription>();

        public AniMediaDbContext(DbContextOptions<AniMediaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AniMediaDbContext).Assembly);
        }
    }
