using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Connectors;

public class OllamaConnector(AppSettings settings) : LanguageModelConnector(settings.Ollama)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as OllamaSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: Ollama.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: Ollama:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: Ollama:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
