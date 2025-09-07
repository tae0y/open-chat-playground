using Microsoft.Extensions.AI;

using OllamaSharp;

namespace OpenChat.ConsoleApp;

class Program
{
    static async Task Main()
    {
        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri("http://localhost:11434"),
            Model = "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF"
        };
        var chatClient = new OllamaApiClient(config);

        var models = await chatClient.ListLocalModelsAsync();
        foreach (var model in models)
        {
            Console.WriteLine($"Model: {model.Name}, Size: {model.Size}, Full: {model}");
        }

        if (models.Any(m => m.Name == config.Model))
        {
            Console.WriteLine($"Model {config.Model} is available.");
        }
        else
        {
            Console.WriteLine($"Model {config.Model} is not available. Please pull the model first.");
            await foreach (var status in chatClient.PullModelAsync(config.Model))
            {
                Console.WriteLine($"{status?.Percent}% {status?.Status}");
            }
        }

        var messages = new ChatMessage(ChatRole.User, "밤하늘은 무슨 색인가요?");
        var chatOptions = new ChatOptions();
        var currentResponseCancellation = new CancellationTokenSource();

        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, chatOptions, currentResponseCancellation.Token))
        {
            Console.Write(update.Text);
        }
    }
}