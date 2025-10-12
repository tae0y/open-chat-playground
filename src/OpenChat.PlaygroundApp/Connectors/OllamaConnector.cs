using Microsoft.Extensions.AI;

using OllamaSharp;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

/// <summary>
/// This represents the connector entity for Ollama.
/// </summary>
public class OllamaConnector(AppSettings settings) : LanguageModelConnector(settings.Ollama)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));
    /// <inheritdoc/>
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as OllamaSettings;
        if (settings is null)
        {
            throw new InvalidOperationException("Missing configuration: Ollama.");
        }

        if (string.IsNullOrWhiteSpace(settings.BaseUrl!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Ollama:BaseUrl.");
        }

        if (string.IsNullOrWhiteSpace(settings.Model!.Trim()) == true)
        {
            throw new InvalidOperationException("Missing configuration: Ollama:Model.");
        }

        return true;
    }

    /// <inheritdoc/>
    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as OllamaSettings;
        var baseUrl = settings!.BaseUrl!;
        var model = settings!.Model!;

        var config = new OllamaApiClient.Configuration
        {
            Uri = new Uri(baseUrl),
            Model = model,
        };

        var chatClient = new OllamaApiClient(config);
        var pulls = chatClient.PullModelAsync(model);
        await foreach (var pull in pulls)
        {
            Console.WriteLine($"Pull status: {pull!.Status}");
        }

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");
        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}