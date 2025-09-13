using System.ClientModel;

using Azure.AI.OpenAI;
using Azure.Identity;

using Microsoft.Extensions.AI;

using Mscc.GenerativeAI.Microsoft;

using OllamaSharp;

using OpenAI;
using OpenAI.Embeddings;

namespace OpenChat.ConsoleApp;

class EmbeddingGeneratorFactory
{
    public static async Task<IEmbeddingGenerator<string,Embedding<float>>> CreateEmbeddingClient(string clientType)
    {
        return clientType switch
        {
            // AzureOpenAI, OpenAI, LGExaone(Ollama/HuggingFace), UpstageSolar(OpenAI), NaverHyperClova(OpenAI)
            "AzureOpenAI" => CreateAzureOpenAIClient(),
            "OpenAI" => CreateOpenAIClient(),
            "LGExaone" => await CreateLGExaoneClientAsync(),
            "UpstageSolar" => CreateUpstageSolarClient(),
            "NaverHyperClova" => CreateNaverHyperClovaClient(),
            "Google" => CreateGoogleClient(),
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
    public static IEmbeddingGenerator<string,Embedding<float>> CreateAzureOpenAIClient()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "";
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "";
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName))
        {
            throw new InvalidOperationException("Azure OpenAI endpoint or deployment name is not set.");
        }
        IEmbeddingGenerator<string,Embedding<float>> generator = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential())
                                    .GetEmbeddingClient(deploymentName)
                                    .AsIEmbeddingGenerator();
        return generator;
    }

    /// <summary>
    /// Creates and configures an OllamaApiClient for HuggingFace models.
    /// </summary>
    /// <returns></returns>
    public static async Task<IEmbeddingGenerator<string,Embedding<float>>> CreateLGExaoneClientAsync()
    {
        // Initialize OllamaApiClient with configuration
        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(Environment.GetEnvironmentVariable("LGEXAONE_URI") ?? "http://localhost:11434"),
            Model = Environment.GetEnvironmentVariable("LGEXAONE_MODEL") ?? "hf.co/LGAI-EXAONE/EXAONE-4.0-1.2B-GGUF"
        };
        var generator = new OllamaApiClient(config);

        // Check if the model is available locally, if not, pull the model
        var models = await generator.ListLocalModelsAsync();
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
            await foreach (var status in generator.PullModelAsync(config.Model))
            {
                Console.WriteLine($"{status?.Percent}% {status?.Status}");
            }
        }

        // Return the configured chat client
        return generator;
    }

    /// <summary>
    /// Creates and configures an OpenAIClient.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IEmbeddingGenerator<string,Embedding<float>> CreateOpenAIClient()
    {
        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
        }

        IEmbeddingGenerator<string,Embedding<float>> generator = new OpenAI.Embeddings.EmbeddingClient(
            model,
            apiKey
        ).AsIEmbeddingGenerator();
        return generator;
    }

    /// <summary>
    /// Creates and configures an UpstageSolarClient.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// To use Upstage's API, ensure you have the UPSTAGE_API_KEY environment variable set with your API key.
    /// You can obtain an API key by signing up on the Upstage platform (https://console.upstage.ai).
    /// </remarks>
    public static IEmbeddingGenerator<string,Embedding<float>> CreateUpstageSolarClient()
    {
        var model = Environment.GetEnvironmentVariable("UPSTAGE_MODEL") ?? "solar-mini";
        var apiKey = Environment.GetEnvironmentVariable("UPSTAGE_API_KEY") ?? "";
        var endpoint = Environment.GetEnvironmentVariable("UPSTAGE_ENDPOINT") ?? "https://api.upstage.ai/v1";
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
        {
            throw new InvalidOperationException("UPSTAGE_API_KEY or Upstage endpoint is not set.");
        }

        var credential = new ApiKeyCredential(apiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint)
        };

        IEmbeddingGenerator<string,Embedding<float>> client = new OpenAI.Embeddings.EmbeddingClient(
            model: model,
            credential: credential,
            options: options
        ).AsIEmbeddingGenerator();
        return client;
    }


    /// <summary>
    /// Creates and configures an NaverHyperClovaClient.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// To use Naver HyperCLOVA, ensure you have the NAVER_CLIENT_ID and NAVER_CLIENT_SECRET environment variables set.
    public static IEmbeddingGenerator<string,Embedding<float>> CreateNaverHyperClovaClient()
    {
        var model = Environment.GetEnvironmentVariable("NAVER_MODEL") ?? "HCX-DASH-001";
        var apiKey = Environment.GetEnvironmentVariable("NAVER_API_KEY") ?? "";
        var endpoint = Environment.GetEnvironmentVariable("NAVER_ENDPOINT") ?? "https://clovastudio.stream.ntruss.com/v1/openai/";
        Console.WriteLine($"Using Naver HyperCLOVA model: {model}, endpoint: {endpoint}");
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
        {
            throw new InvalidOperationException("NAVER_API_KEY or NAVER_ENDPOINT is not set.");
        }

        var credential = new ApiKeyCredential(apiKey);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(endpoint)
        };

        IEmbeddingGenerator<string,Embedding<float>> client = new OpenAI.Embeddings.EmbeddingClient(
            model: model,
            credential: credential,
            options: options
        ).AsIEmbeddingGenerator();
        return client;
    }

    /// <summary>
    /// Creates and configures a Google PaLM2 Client.
    /// </summary>
    public static IEmbeddingGenerator<string,Embedding<float>> CreateGoogleClient()
    {
        var apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? "";
        var model = Environment.GetEnvironmentVariable("GOOGLE_MODEL") ?? "gemini-1.5-pro";
        IEmbeddingGenerator<string,Embedding<float>> generator = new GeminiEmbeddingGenerator(apiKey, model);
        return generator;
    }
}
