using YummyAnimeNotifier.Application.Worker;
using YummyAnimeNotifier.Infrastructure.External.MassTransit;
using YummyAnimeNotifier.Infrastructure.External.Telegram;
using YummyAnimeNotifier.Infrastructure.External.YummyAnime;
using YummyAnimeNotifier.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddWorkerApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddYummyAnime(builder.Configuration)
    .AddMassTransit(builder.Configuration, registerConsumers: false);
// TODO: move to separate bot project
// .AddTelegramReceiving(builder.Configuration);

builder.Logging.AddFilter("MassTransit", LogLevel.Debug);

var host = builder.Build();
host.Run();