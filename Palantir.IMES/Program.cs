using Armali.Horizon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Palantir.Core.Services;
using Palantir.IMES.Services;

namespace Palantir.IMES
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var horizon = new Horizon(args);
            horizon.Initialize()
                   .AddLogging()
                   .AddMessaging();

            // Register services
            horizon.Services.AddSingleton<ImesService>();
            horizon.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<ImesService>());
            horizon.Services.AddSingleton<AzureOpenAIService>();
            horizon.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<AzureOpenAIService>());
            //

            var host = horizon.Build();
            await host.RunAsync(horizon);
        }
    }
}
