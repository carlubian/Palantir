using Armali.Horizon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            horizon.Services.AddSingleton<IHostedService, ImesService>();
            //

            var host = horizon.Build();
            await host.RunAsync(horizon);
        }
    }
}
