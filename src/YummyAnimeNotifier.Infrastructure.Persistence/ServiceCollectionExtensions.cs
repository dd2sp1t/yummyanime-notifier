using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YummyAnimeNotifier.Infrastructure.Persistence.ConflictDetectors;
using MediatR;
using YummyAnimeNotifier.Infrastructure.Persistence.PipelineBehaviors;

namespace YummyAnimeNotifier.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var provider = configuration.GetValue<string>("Database:Provider");

            services.AddDbContext<YummyAnimeDbContext>(options =>
            {
                switch (provider)
                {
                    case "Sqlite":
                        options.UseSqlite(configuration.GetConnectionString("Sqlite"));
                        break;

                    case "Postgres":
                        options.UseNpgsql(configuration.GetConnectionString("Postgres"));
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unknown database provider: {provider}");
                }
            });

            switch (provider)
            {
                case "Sqlite":
                    services.AddScoped<IConflictDetector, SqliteConflictDetector>();
                    break;

                case "Postgres":
                    services.AddScoped<IConflictDetector, PgSqlConflictDetector>();
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unknown database provider: {provider}");
            }

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAnimeRepository, AnimeRepository>();
            services.AddScoped<IAnimeTranslationRepository, AnimeTranslationRepository>();
            services.AddScoped<ITranslationSourceRepository, TranslationSourceRepository>();
            services.AddScoped<IReleaseRepository, ReleaseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ConcurrencyRetryBehavior<,>));

            return services;
        }
    }
}