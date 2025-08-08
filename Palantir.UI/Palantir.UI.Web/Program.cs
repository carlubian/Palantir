using Armali.Horizon.Logs;
using Armali.Horizon.Messaging;
using Palantir.Core;
using Palantir.UI.Web;
using Palantir.UI.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new("http://apiservice"));

var ilogger = Utils.ProduceLogger();
builder.Services.AddSingleton<IHorizonMessaging>(provider =>
{
    var Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

    var logService = provider.GetRequiredService<IHorizonLogger>();
    var msgService = new HorizonMessaging(logService);
    var endpoint = Configuration["Horizon:Messaging:Endpoint"] ?? "localhost:6400";
    msgService.SetConnection(endpoint);
    msgService.SetComponent(Configuration["Horizon:Component"] ?? "Horizon");

    return msgService;
});
builder.Services.AddSingleton<IHostedService, IHorizonMessaging>(provider => provider.GetRequiredService<IHorizonMessaging>());
builder.Services.AddSingleton<IHorizonLogger>(new HorizonLogger(ilogger));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
