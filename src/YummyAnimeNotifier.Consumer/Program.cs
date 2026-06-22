using YummyAnimeNotifier.Application.Consumer;
using YummyAnimeNotifier.Infrastructure.External.MassTransit;
using YummyAnimeNotifier.Infrastructure.External.Telegram;
using YummyAnimeNotifier.Infrastructure.External.YummyAnime;
using YummyAnimeNotifier.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddConsumerApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddYummyAnime(builder.Configuration)
    .AddTelegramSending(builder.Configuration)
    .AddMassTransit(builder.Configuration, registerConsumers: true);

builder.Logging.AddFilter("MassTransit", LogLevel.Debug);

var host = builder.Build();
host.Run();
