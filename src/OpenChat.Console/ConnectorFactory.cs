using System.ClientModel;

using Azure.AI.OpenAI;
using Azure.Identity;

using Microsoft.Extensions.AI;
using OllamaSharp;

using OpenAI;
using OpenAI.Chat;

namespace OpenChat.ConsoleApp;

class ConnectorFactory
{
    public static async Task<IChatClient> CreateChatClient(string clientType)
    {
        return clientType switch
        {
            "HuggingFace" => await CreateHuggingFaceClientAsync(),
            "OpenAI" => CreateOpenAIClient(),
            "AzureOpenAI" => CreateAzureOpenAIClient(),
            "UpstageSolar" => CreateUpstageSolarClient(),
            _ => throw new ArgumentException("Invalid client type")
        };
    }

    /// <summary>
    /// Creates and configures an AzureOpenAIClient.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    /// To use Azure OpenAI, ensure you login via Azure CLI to proper subscription.
    /// You can deploy models via Azure Portal, Azure AI Foundry, and so on.
    /// </remarks>
    public static IChatClient CreateAzureOpenAIClient()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "";
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "";
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName))
        {
            throw new InvalidOperationException("Azure OpenAI endpoint or deployment name is not set.");
        }
        IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
                                    .GetChatClient(deploymentName)
                                    .AsIChatClient();
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
            Uri = new Uri(Environment.GetEnvironmentVariable("HUGGINGFACE_URI") ?? "http://localhost:11434"),
            Model = Environment.GetEnvironmentVariable("HUGGINGFACE_MODEL") ?? "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF"
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

    /// <summary>
    /// Creates and configures an OpenAIClient.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IChatClient CreateOpenAIClient()
    {
        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
        }

        var chatClient = new OpenAI.Chat.ChatClient(
            model,
            apiKey
        ).AsIChatClient();
        return chatClient;
    }

    /// <summary>
    /// Creates and configures an UpstageSolarClient.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// To use Upstage's API, ensure you have the UPSTAGE_API_KEY environment variable set with your API key.
    /// You can obtain an API key by signing up on the Upstage platform (https://console.upstage.ai).
    /// </remarks>
    public static IChatClient CreateUpstageSolarClient()
    {
        var model = Environment.GetEnvironmentVariable("UPSTAGE_MODEL") ?? "solar-mini";
        var apiKey = Environment.GetEnvironmentVariable("UPSTAGE_API_KEY") ?? "";
        var endpoint = "https://api.upstage.ai/v1";
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
        {
            throw new InvalidOperationException("UPSTAGE_API_KEY or Upstage endpoint is not set.");
        }

        var credential = new ApiKeyCredential(apiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint)
        };

        IChatClient client = new OpenAI.Chat.ChatClient(
            model: model,
            credential: credential,
            options: options
        ).AsIChatClient();
        return client;
    }

}
