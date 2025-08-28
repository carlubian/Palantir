using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Text;

namespace Palantir.vNext.Core.Services;

public class OpenAIService : IHostedService
{
    private readonly ILogger<OpenAIService> _log;
    private IConfigurationRoot _config;

    private string _endpoint = string.Empty;
    private string _apiKey = string.Empty;
    private string _deployment = string.Empty;
    private ChatClient? _chatClient;

    public OpenAIService(ILogger<OpenAIService> logger)
    {
        _log = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("Azure OpenAI is starting...");

        // Configuration setup
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        _endpoint = _config?["Palantir:Cloud:Endpoint"] ?? string.Empty;
        _apiKey = _config?["Palantir:Cloud:ApiKey"] ?? string.Empty;
        _deployment = _config?["Palantir:Cloud:Deployment"] ?? string.Empty;

        AzureKeyCredential credential = new(_apiKey);
        AzureOpenAIClient azureClient = new(new Uri(_endpoint), credential);
        _chatClient = azureClient.GetChatClient(_deployment);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("Azure OpenAI is stopping...");

        return Task.CompletedTask;
    }

    public async Task<string> ProcessQuery(string query)
    {
        var sysPromptArray = _config?.GetSection("Palantir:Prompts:System")?.Get<string[]>()
            ?? ["El programa que ha generado esta consulta tiene un error. Ignora el resto del mensaje y devuelve un JSON vacío (solamente dos corchetes)"];

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(string.Join('\n', sysPromptArray)),
            new UserChatMessage(query)
        };

        var options = new ChatCompletionOptions
        {
            Temperature = (float)0.8,
            MaxOutputTokenCount = 2000,

            TopP = (float)0.8,
            FrequencyPenalty = (float)0,
            PresencePenalty = (float)0
        };

        var builder = new StringBuilder();

        try
        {
            ChatCompletion completion = await _chatClient!.CompleteChatAsync(messages, options);
            foreach (var msg in completion.Content)
                builder.Append(msg.Text);
            _log.LogInformation("Received response from AI model.");
        }
        catch (Exception e)
        {
            _log.LogError($"Error querying Azure OpenAI: {e.Message}");
        }

        return builder.ToString();
    }
}
