using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

                    // case "Postgres":
                    //     options.UseNpgsql(configuration.GetConnectionString("Postgres"));
                    //     break;

                    default:
                        throw new InvalidOperationException(
                            $"Unknown database provider: {provider}");
                }
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAnimeRepository, AnimeRepository>();
            services.AddScoped<IAnimeTranslationRepository, AnimeTranslationRepository>();
            services.AddScoped<ITranslationSourceRepository, TranslationSourceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

            return services;
        }
    }
}