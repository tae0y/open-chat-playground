using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;

namespace OpenChat.PlaygroundApp.Connectors;

public class AnthropicConnector(AppSettings settings) : LanguageModelConnector(settings.Anthropic)
{
    public override bool EnsureLanguageModelSettingsValid()
    {
        var settings = this.Settings as AnthropicSettings;
        if (settings is null)
            throw new InvalidOperationException("Missing configuration: Anthropic.");
        if (string.IsNullOrWhiteSpace(settings.Endpoint))
            throw new InvalidOperationException("Missing configuration: Anthropic:Endpoint.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("Missing configuration: Anthropic:ApiKey.");
        if (string.IsNullOrWhiteSpace(settings.Model))
            throw new InvalidOperationException("Missing configuration: Anthropic:Model.");
        return true;
    }

    public override Task<IChatClient> GetChatClientAsync()
    {
        throw new NotImplementedException();
    }
}
