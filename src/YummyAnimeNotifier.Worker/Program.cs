using YummyAnimeNotifier.Application;
using YummyAnimeNotifier.Infrastructure.External;
using YummyAnimeNotifier.Infrastructure.Persistence;
using YummyAnimeNotifier.Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddExternal(builder.Configuration);

builder.Logging.AddFilter("MassTransit", LogLevel.Debug);

// builder.Services.AddHostedService<EpisodeTrackingBackgroundService>();
builder.Services.AddHostedService<OutboxPublisherBackgroundService>();

var host = builder.Build();
host.Run();
