using Microsoft.Extensions.AI;
using OllamaSharp;

namespace OpenChat.ConsoleApp;

class ConnectorFactory
{
    public static async Task<IChatClient> CreateChatClient(string clientType)
    {
        return clientType switch
        {
            "HuggingFace" => await CreateHuggingFaceClientAsync(),
            _ => throw new ArgumentException("Invalid client type")
        };
    }

    /// <summary>
    /// Creates and configures an OpenAIClient.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IChatClient> CreateOpenAIClientAsync()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
        }
        var chatClient = new OpenAIClient(apiKey);
        return chatClient;
    }

    /// <summary>
    /// Creates and configures an AzureOpenAIClient.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<IChatClient> CreateAzureOpenAIClientAsync()
    {
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName))
        {
            throw new InvalidOperationException("One or more Azure OpenAI environment variables are not set.");
        }

        var chatClient = new AzureOpenAIClient(endpoint, apiKey, deploymentName);
        return chatClient;
    }

    /// <summary>
    /// Creates and configures an OllamaApiClient for HuggingFace models.
    /// </summary>
    /// <returns></returns>
    public static async Task<IChatClient> CreateHuggingFaceClientAsync()
    {
        // Initialize OllamaApiClient with configuration
        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri("http://localhost:11434"),
            Model = "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF"
        };
        var chatClient = new OllamaApiClient(config);

        // Check if the model is available locally, if not, pull the model
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

        // Return the configured chat client
        return chatClient;
    }


}
