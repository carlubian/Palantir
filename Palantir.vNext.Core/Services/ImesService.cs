using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Palantir.vNext.Core.Model;
using System.Text;
using System.Text.Json;

namespace Palantir.vNext.Core.Services;

public class ImesService : IHostedService
{
    private readonly ILogger<ImesService> _log;
    private readonly OpenAIService _ai;

    public ImesService(ILogger<ImesService> logger, OpenAIService ai)
    {
        _log = logger;
        _ai = ai;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("IMES is starting...");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("IMES is stopping...");

        return Task.CompletedTask;
    }

    public async Task<Hypothesis> ProcessConversation(Conversation conversation)
    {
        var builder = new StringBuilder();

        foreach (var entry in conversation.Turns)
        {
            builder.AppendLine($"{entry.Speaker}: {entry.Content}");
            builder.AppendLine();
        }

        _log.LogInformation("Sending serialized conversation to model");
        var response = await _ai.ProcessQuery(builder.ToString());

        _log.LogInformation("Trying to deserialize model response");
        var output = JsonSerializer.Deserialize<Hypothesis>(response);

        return output;
    }
}
