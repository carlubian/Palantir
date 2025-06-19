using Armali.Horizon.Logs;
using Armali.Horizon.Messaging;
using Armali.Horizon.Messaging.Model;
using Microsoft.Extensions.Hosting;
using Palantir.Core.Model;
using Palantir.Core.Services;

namespace Palantir.IMES.Services;

internal class ImesService(IHorizonLogger log, IHorizonMessaging messaging, AzureOpenAIService openai) : IHostedService
{
    private readonly IHorizonLogger _log = log;
    private readonly IHorizonMessaging _messaging = messaging;
    private readonly AzureOpenAIService _openai = openai;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _log.Info("IMES is starting...");
        _messaging.OnMessageReceived += IncomingMessage;
        _messaging.Listen("Conversation");

        new Thread(async () => { Thread.Sleep(4000); await SendTestMessage(); }).Start(); // Only for test purposes

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _log.Info("IMES is stopping...");

        return Task.CompletedTask;
    }

    private void IncomingMessage(MessagePayload payload)
    {
        _log.Trace($"New {payload.TypeHint} message received");

        // For the test phase, use the debug message type
        var message = payload.Deserialize<TestMessage>();

        if (message is null)
        {
            _log.Warning("Received message is null or not of type TestMessage");
            SendTestMessage().Wait(); // This is only for the test phase
            return;
        }

        _log.Info($"Received message: {message.ModelQuery}");
        _log.Info("Querying Azure OpenAI with the provided message...");

        // Make the query to Azure OpenAI
        var task = _openai.QueryModel(message.ModelQuery);

        task.Wait();
        var response = task.Result;

        _log.Info($"Response from Azure OpenAI: {response}");
    }

    public async Task SendTestMessage()
    {
        // Send test message to verify the service is running
        var payload = new TestMessage
        {
            ModelQuery = "¿Cuáles son las líneas aéreas que forman parte de la alianza OneWorld?"
        };
        await _messaging.SendMessage("Conversation", payload);
    }
}
