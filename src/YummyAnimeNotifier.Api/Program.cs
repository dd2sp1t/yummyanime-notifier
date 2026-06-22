using System.Text.Json.Serialization;
using YummyAnimeNotifier.Application.Consumer;
using YummyAnimeNotifier.Infrastructure.External.YummyAnime;
using YummyAnimeNotifier.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddConsumerApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddYummyAnime(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// app.UseHttpsRedirection();

app.Run();