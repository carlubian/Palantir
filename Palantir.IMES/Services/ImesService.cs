using Armali.Horizon.Logs;
using Armali.Horizon.Messaging;
using Armali.Horizon.Messaging.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Palantir.Core.Model;
using Palantir.Core.Services;

namespace Palantir.IMES.Services;

internal class ImesService(IHorizonLogger log, IHorizonMessaging messaging) : IHostedService
{
    private readonly IHorizonLogger _log = log;
    private readonly IHorizonMessaging _messaging = messaging;
    private IConfigurationRoot? _config;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _log.Info("IMES is starting...");
        _messaging.OnMessageReceived += IncomingMessage;
        _messaging.Listen("Conversation");

        // Temporary configuration setup
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // Send test message to verify the service is running
        var payload = new TestMessage
        {
            ModelQuery = "¿Cuáles son las líneas aéreas que forman parte de la alianza OneWorld?"
        };
        await _messaging.SendMessage("Conversation", payload);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _log.Info("IMES is stopping...");
    }

    private void IncomingMessage(MessagePayload payload)
    {
        _log.Trace($"New {payload.TypeHint} message received");

        // For the test phase, use the debug message type
        var message = payload.Deserialize<TestMessage>();

        if (message is null)
        {
            _log.Warning("Received message is null or not of type TestMessage");
            return;
        }

        _log.Info($"Received message: {message.ModelQuery}");
        _log.Info("Querying Azure OpenAI with the provided message...");

        // Make the query to Azure OpenAI
        var task = AzureOpenAIService.QueryModel(
            _config?["Palantir:Cloud:Endpoint"] ?? string.Empty,
            _config?["Palantir:Cloud:ApiKey"] ?? string.Empty,
            _config?["Palantir:Cloud:Deployment"] ?? string.Empty,
            message.ModelQuery);

        task.Wait();
        var response = task.Result;

        _log.Info($"Response from Azure OpenAI: {response}");
    }
}
