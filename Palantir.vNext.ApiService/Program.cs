using Microsoft.AspNetCore.ResponseCompression;
using Palantir.vNext.ApiService.Hubs;
using Palantir.vNext.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<OpenAIService>();
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<OpenAIService>());
builder.Services.AddSingleton<ImesService>();
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<ImesService>());

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

var app = builder.Build();

app.UseResponseCompression();
app.MapHub<PalantirHub>("/palantir");

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.Run();
