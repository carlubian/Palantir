using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text;

namespace Palantir.Core.Services;

// For now this is a regular class, IHostedService will be implemented later
public class AzureOpenAIService
{

    public static async Task<string> QueryModel(string endpoint, string apiKey, string deployment, string query)
    {
        AzureKeyCredential credential = new AzureKeyCredential(apiKey);
        AzureOpenAIClient azureClient = new(new Uri(endpoint), credential);
        ChatClient chatClient = azureClient.GetChatClient(deployment);

        // Crear una lista de mensajes de chat
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(@"Es un asistente de inteligencia artificial que ayuda a los usuarios a encontrar información."),
            new UserChatMessage(query)
        };

        var options = new ChatCompletionOptions
        {
            Temperature = (float)1,
            MaxOutputTokenCount = 1000,

            TopP = (float)1,
            FrequencyPenalty = (float)0,
            PresencePenalty = (float)0
        };

        ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);
        var builder = new StringBuilder();
        foreach (var msg in completion.Content)
            builder.Append(msg.Text);

        return builder.ToString();
    }
}
