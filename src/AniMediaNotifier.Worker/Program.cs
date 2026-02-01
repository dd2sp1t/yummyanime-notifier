using AniMediaNotifier.Application;
using AniMediaNotifier.Infrastructure.External;
using AniMediaNotifier.Infrastructure.Persistence;
using AniMediaNotifier.Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddExternal(builder.Configuration);

builder.Services.AddHostedService<EpisodeTrackingHostedService>();

var host = builder.Build();
host.Run();
